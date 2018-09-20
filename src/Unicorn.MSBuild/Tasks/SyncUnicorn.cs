namespace Unicorn.MSBuild.Tasks
{
    using MicroCHAP;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System.IO;
    using System.Net;

    namespace Unicorn.MSBuild.Tasks
    {
        public class SyncUnicorn : Task
        {
            [Required]
            public string ControlPanelUrl { get; set; }

            [Required]
            public string SharedSecret { get; set; }

            public override bool Execute()
            {
                var url = $"{ControlPanelUrl}?verb=Sync&configuration=&skipTransparentConfigs=false";

                var signatureService = new SignatureService(SharedSecret);
                var challenge = GetChallenge();
                var signature = signatureService.CreateSignature(challenge, url, null);

                return ExecuteUnicornSync(signature, url, challenge) && !Log.HasLoggedErrors;
            }

            private bool ExecuteUnicornSync(SignatureResult signature, string url, string challenge)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var request = WebRequest.Create(url);
                request.Headers["X-MC-MAC"] = signature.SignatureHash;
                request.Headers["X-MC-Nonce"] = challenge;
                request.Timeout = 10800000; // 3 hours

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var index = line?.IndexOf(':');

                        if (index >= 0)
                        {
                            var type = line.Substring(0, index.Value);
                            var message = line.Substring(index.Value);
                            switch (type)
                            {
                                case "Error":
                                    Log.LogError(message);
                                    break;
                                case "Warning":
                                    Log.LogWarning(message);
                                    break;
                                default:
                                    Log.LogMessage(message);
                                    break;
                            }
                        }
                        else
                        {
                            Log.LogMessage(line);
                        }

                        if (line != null && line.Contains("****ERROR OCCURRED****"))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            private string GetChallenge()
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var challengeRequest = WebRequest.Create($"{ControlPanelUrl}?verb=Challenge");
                challengeRequest.Timeout = 360000;

                using (var response = challengeRequest.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}