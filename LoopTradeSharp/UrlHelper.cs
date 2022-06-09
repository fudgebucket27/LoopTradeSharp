using PoseidonSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace LoopTradeSharp
{
    public static class UrlHelper
    {
        public static string Sign(string layerTwoPrivateKey, HttpMethod method, List<(string Key, string Value)> queryParameters, string postBody, string apiMethod, string apiUrl)
        {
            var signatureBase = "";
            var parameterString = "";
            if (method == HttpMethod.Post)
            {
                signatureBase += "POST&";
                parameterString = postBody;
            }
            else if (method == HttpMethod.Get)
            {
                signatureBase += "GET&";
                if (queryParameters != null)
                {
                    int i = 0;
                    foreach (var parameter in queryParameters)
                    {
                        parameterString += parameter.Key + "=" + parameter.Value;
                        if (i < queryParameters.Count - 1)
                            parameterString += "&";
                        i++;
                    }
                }
            }
            else if (method == HttpMethod.Delete)
            {
                signatureBase += "DELETE&";
                if (queryParameters != null)
                {
                    int i = 0;
                    foreach (var parameter in queryParameters)
                    {
                        parameterString += parameter.Key + "=" + parameter.Value;
                        if (i < queryParameters.Count - 1)
                            parameterString += "&";
                        i++;
                    }
                }
            }
            else
                throw new Exception("Http method type not supported");

            signatureBase += UrlEncodeUpperCase(apiUrl + apiMethod) + "&";
            signatureBase += UrlEncodeUpperCase(parameterString);

            var message = SHA256Helper.CalculateSHA256HashNumber(signatureBase);

            var signer = new Eddsa(message, layerTwoPrivateKey);
            return signer.Sign();
        }
        public static string UrlEncodeUpperCase(string stringToEncode)
        {
            var reg = new Regex(@"%[a-f0-9]{2}");
            stringToEncode = HttpUtility.UrlEncode(stringToEncode);
            return reg.Replace(stringToEncode, m => m.Value.ToUpperInvariant());
        }
    }
}
