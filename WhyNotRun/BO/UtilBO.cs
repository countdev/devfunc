using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace WhyNotRun.BO
{
    public static class UtilBO
    {
        public const string OBJECT_ID_REGEX = "^[0-9a-fA-F]{24}$";
        public const int QUANTIDADE_PAGINAS = 20;


        public static string ValorAuthorizationHeader(HttpContext httpContext)
        {
            var header = httpContext.Request.Headers["Authorization"];
            if (header != null)
            {
                return header;
            }
            return null;
        }

        public static ObjectId ToObjectId(this string valor)
        {
            if (ValidarRegex(valor, OBJECT_ID_REGEX))
            {
                return new ObjectId(valor);
            }
            throw new InvalidCastException("A string à ser convertida não é um ObjectId válido.");
        }

        public static bool ValidarRegex(string valor, string pattern)
        {
            var match = Regex.Match(valor, pattern, RegexOptions.IgnoreCase);
            return match.Success;
        }

        public static string Encript(this string text)
        {
            using (var sha512 = SHA512Managed.Create())
            {
                var valueWithSalt = $"2eae240d4da36a1841ac5742347b85ce74de465b5b42c902d67{text}21611c15bf2a94ccf879c95293cc9b760a26746f0e71d4f90b8c49efa3f4622354a92ce006904";
                sha512.ComputeHash(ASCIIEncoding.UTF8.GetBytes(valueWithSalt));
                return BitConverter.ToString(sha512.Hash).Replace("-", "").ToLower();
            }
        }
        
        //public static string SaveImage()
        //{
            
        //}

    }
}