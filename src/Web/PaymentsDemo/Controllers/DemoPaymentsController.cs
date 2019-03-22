namespace PaymentsDemo.Controllers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using BankSystem.Common.Utils;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Newtonsoft.Json;

    public class DemoPaymentsController : Controller
    {
        // These constants should be stored in a configuration file

        private const string PaymentsDemoPrivateKey =
            "<RSAKeyValue><Modulus>3AZV1/LbC/enLZZvsWAgpoMtyISeLhGa/ZASeWztea8qymH/fCaNkPGa0BJy+jbQqTRguuqTgxbUaFizscd75ZpbYi29QeT5v8SUKjjEGlhYy6vlGMOedagGHvc6tmqOECifIhxo8bTYXkxSuH8YoNe6zN7OgM+FHdja6+K85lsXpyINAMMaPQk4QChe57LCNcgJapcbFrGNBleIikWqoyy6tmlaAKq20LTdfRp04VpXpD0lAJ6wJ+KUdHr7H+WPqN5ZGO2PLNfrr19sKjARugUYMoI93K7IARcFANT5uFOXBG1mwkDh+R82k0Z1NuEmGt8T8APXTOmZ4rVL2k+/mQ==</Modulus><Exponent>AQAB</Exponent><P>+0sipj3iIqnwA8vKyuwkVebc+EpbOzCLPCRxUI1Wiie1Rio+KReZTAQbkwLYXY6ECx+KE9AP/eGxto9wgmtwst6keDNdH7al2grV6j4/TAUG1/kezwxTIUeycU3DpdMifI8/CM8HU3jh/2nyr3vqOqbasdFih8huwSMNFkagyrs=</P><Q>4CVG+IDJWEpgvQKwKkz/QgnOvpq44n1Knfc3aeKYelIwizADgQ6sL05kgEG0NQb14v/QjlptWbGjMq99EArYHkFbDoTtcx41jdMdx5GiExOSwvIgz1uGF6NhTkE0VmbcT5YS7qRawsdWG+UYKxz4S3g5N6IQRcACFNZ0oSMm67s=</Q><DP>3VjWYXZ6/SuRFdbpfwq4Cs56712XtLBSxJwZD+ofQzwsyWwmKs31ouavXzQPX4FMP/v9BOytWWT2w6bfZJG8yGGin5omueuJdWE8AcPov05iM9Tk1V22z8a2oGTuI9+xLeSDkn/BpT4CW5d4RCizLgyQ8DZOQupC3G7CdU+rDEc=</DP><DQ>BuC988Qn6YCmxUX+191y+7jHUkv3HmQP1RcP6TzdxuscuDip8tzbZbw3E0Rw3iuvgd6trKCTuGveASEnakWa6hrBS4nCq4Siyg5PXJ9YZNN17mt1nEdHrxQBWWBg1cHkQsDtJct/SXjKaKK4AiKqb85pmw5rB2jj53XJMGSevh8=</DQ><InverseQ>huIIyAW61ucRHMs8A2KOCDZbfuDqSi9O6IjjeZHTakcH7GTuPSe9FT2fS9FamBoUhcEnqrkjyQ3LkrSTeh1Na9OQv60r3BDgXlemo7g7VhjOWka9mF7qcdjhjHPhuK4bg4EytJ3dAACmH6T5rOiuI3V5cY7LKoDRyD/wOSuxgJA=</InverseQ><D>juiSg0PbIKfHbyXCADyGmIClkRMKRrPqrn0QcsvcZufubCtArRvBYX575l/FTF2kI/LATUelbkS6y9epR3RhNd2PM3Kv9YfK69K7xby/KrltW0SX9gDBAHyVFTHRccA966Lm0VoQk8W5r231YJ40mevlCcJB8IpZVOyQLRxFQ3x8TrJ18vmy3KHr+ur+tIGKm/7sXomZHK1uRV4qwhK7tB253sBsSPbtsGiLXDZT4KKvBr+06W4ZQcvOg2cIrvn/C5lmnZvfOSrrV2+YR6XMAmp1oX5VYaaAXHTdezplb9OVPVptbpXpTnzVSflI0QwcT4hDXUs7Q+CIpwR4iqrg/Q==</D></RSAKeyValue>";

        private const string CentralApiPublicKey =
            "<RSAKeyValue><Modulus>v76m9MR9WiAq7W/A9xiAh5rxjm354nnm619Phns5CDtHhf4A4Vos+eqAhqU/+S3jHXZyUmfRLvplvUkXqu25RaUxXNQCrDOoSyPoYzlVJOVNQfvXPlCeHhk/a/owg7wrAe8RwC12dRysT7dgPysA2jmvEDLJjGgSAslbmVXgqFc3wH+XueChA60lyMbu5nvCG+G38qyVQHrFyX0UtlYuLcPfg8qnw/it7SjdlO4oi8iDRDXjMTmpYN48KQYzyH3i9MJW2HNnGs2EHt7+UR5/3cPdsvKOQvO6wq6tgvzb/u6wq2aUITld/X3hKscKhMi18IJgXKBU5xAf677ge4eFWjBY+UMKKXPC1HRFEPdu7qMTjaGQkucI3fx2zf5e5dERyvhjkYROCRR/jJbi7iiEapUVKlLTLWsshA2VtcUz9rrYOj0DwS8s9IeQEIrCqb2xrVEEsyvn9y1GX5oLKZ58Uf+DdW4xe9lLmMZoC46QwPX+jzq+5iolp7a8P2RHsusqoWTLHmsq3fUXufzm7flOe5yTB+84ZsuODqMkis5QNKbnqlDMkkwH190TeftHrBdq5SyPpCAvpQMtk2/ShCkSqbgsoR3gu2ThThmpYQgo/wTx0up4U48NLh68HxqZs9A9glOKlNWm2MTKnvRkmCRssYRit099YxaG6zrgfwn1VME=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private const string ReturnUrl = "https://localhost:6001/DemoPayments/ReceivePaymentConfirmation?data={0}";
        private const string CentralApiUrl = "http://localhost:5000/pay/{0}";

        [HttpPost]
        public IActionResult CreatePaymentRequest(CreateDemoPaymentBindingModel model)
        {
            var paymentInfo = new
            {
                model.Amount,
                model.Description,
                model.DestinationBankName,
                model.DestinationBankCountry,
                model.DestinationBankSwiftCode,
                model.DestinationBankAccountUniqueId,

                // ! PaymentInfo can also contain custom properties
                // ! that will be returned on payment completion

                // ! PaymentId is a custom property and is not required
                PaymentId = Guid.NewGuid()
            };

            // ! The payment should be persisted in the database as incomplete

            var paymentInfoJson = JsonConvert.SerializeObject(paymentInfo);

            string paymentInfoSignature;
            string websitePublicKey;

            // sign the PaymentInfo for verification in ReceivePaymentResult
            // and include the public key of the merchant website for signature verification

            // the public key and signature could be modified by an attacker,
            // but this will be detected in ReceivePaymentResult
            using (var websiteRsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(websiteRsa, PaymentsDemoPrivateKey);

                paymentInfoSignature = Convert.ToBase64String(
                    websiteRsa.SignData(
                        Encoding.UTF8.GetBytes(paymentInfoJson),
                        HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));

                websitePublicKey = RsaExtensions.ToXmlString(websiteRsa, false);
            }

            var paymentRequest = new
            {
                PaymentInfo = paymentInfoJson,
                PaymentInfoSignature = paymentInfoSignature,
                PublicKey = websitePublicKey,
                ReturnUrl
            };

            string paymentRequestJson = JsonConvert.SerializeObject(paymentRequest);

            string data = Base64UrlUtil.Encode(paymentRequestJson);

            return this.Redirect(string.Format(CentralApiUrl, data));
        }

        [HttpGet]
        public IActionResult ReceivePaymentConfirmation(string data)
        {
            try
            {
                string decodedJson = Base64UrlUtil.Decode(data);


                // deserialize json
                dynamic deserializedJson = JsonConvert.DeserializeObject(decodedJson);

                string paymentInfoJson = deserializedJson.PaymentInfo;
                string paymentProofJson = deserializedJson.PaymentProof;
                string paymentConfirmationJson = deserializedJson.PaymentConfirmation;
                string bankPaymentConfirmationSignature = deserializedJson.PaymentConfirmationSignature;

                dynamic paymentInfo = JsonConvert.DeserializeObject(paymentInfoJson);
                dynamic paymentProof = JsonConvert.DeserializeObject(paymentProofJson);
                dynamic paymentConfirmation = JsonConvert.DeserializeObject(paymentConfirmationJson);

                string bankPublicKey = paymentProof.BankPublicKey;
                string paymentInfoSignature = paymentProof.PaymentInfoSignature;

                string paymentProofSignature = paymentConfirmation.PaymentProofSignature;
                bool success = paymentConfirmation.Success;


                // Verify the payment confirmation trust chain
                // (the central api provides us with the bank's key)

                // verify the PaymentInfo to make sure it has not been modified before being processed by the bank
                bool isPaymentInfoSignatureValid = VerifySignature(paymentInfoJson,
                    paymentInfoSignature, PaymentsDemoPrivateKey);

                if (!isPaymentInfoSignatureValid)
                {
                    return this.BadRequest("PaymentInfo signature is invalid");
                }

                // verify the PaymentProof to make sure the provided bank key is trusted by the central api
                bool isPaymentProofSignatureValid = VerifySignature(paymentProofJson,
                    paymentProofSignature, CentralApiPublicKey);

                if (!isPaymentProofSignatureValid)
                {
                    return this.BadRequest("PaymentProof signature is invalid");
                }

                // finally, verify the PaymentConfirmation using the (now trusted) bank's key
                bool isPaymentConfirmationSignatureValid = VerifySignature(paymentConfirmationJson,
                    bankPaymentConfirmationSignature, bankPublicKey);

                if (!isPaymentConfirmationSignatureValid)
                {
                    return this.BadRequest("PaymentConfirmation signature is invalid");
                }

                // check if the payment is successfully completed
                if (success != true)
                {
                    return this.BadRequest("Payment was unsuccessful");
                }

                /*
                 * !!
                 * !! TODO! It should now be checked in the database that this payment has not already been completed
                 * !!
                 * !! (a malicious user could send the same payment confirmation more than once
                 * !!  in order to make us think they have bought something multiple times)
                 * !!
                 */


                /*
                 * Optionally, additional checks may also be performed using the data in the PaymentInfo
                 *
                 * 
                 * !! TODO! If all checks pass, the payment should be marked as completed in the database
                 * 
                 */


                // for demonstration purposes, we will simply show the payment details
                var model = new PaymentDetailsViewModel
                {
                    Amount = paymentInfo.Amount,
                    Description = paymentInfo.Description,
                    PaymentId = paymentInfo.PaymentId
                };

                return this.View(model);
            }
            catch
            {
                return this.BadRequest("An error occured while processing payment result");
            }
        }

        private static bool VerifySignature(string data, string signature, string key)
        {
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, key);

                bool isSignatureValid = rsa.VerifyData(
                    Encoding.UTF8.GetBytes(data),
                    Convert.FromBase64String(signature),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                return isSignatureValid;
            }
        }
    }
}