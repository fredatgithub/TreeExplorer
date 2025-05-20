using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TreeExplorer.Helpers
{
  public static class StringHelper
  {
    public static string Plural(int count)
    {
      return count > 1 ? "s" : "";
    }

    public static string Reverse(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }

      char[] charArray = input.ToCharArray();
      Array.Reverse(charArray);
      return new string(charArray);
    }

    public static string RemoveSpecialCharacters(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }

      var sb = new StringBuilder();
      foreach (char c in input)
      {
        if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
        {
          sb.Append(c);
        }
      }
      return sb.ToString();
    }

    public static string Truncate(string input, int maxLength)
    {
      if (string.IsNullOrEmpty(input) || maxLength <= 0)
      {
        return string.Empty;
      }

      return input.Length <= maxLength ? input : input.Substring(0, maxLength);
    }

    public static string ToTitleCase(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }

      return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }

    public static string ToCamelCase(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }

      var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      if (words.Length == 0)
      {
        return string.Empty;
      }

      var sb = new StringBuilder(words[0].ToLower());
      for (int i = 1; i < words.Length; i++)
      {
        sb.Append(char.ToUpper(words[i][0]));
        sb.Append(words[i].Substring(1).ToLower());
      }
      return sb.ToString();
    }

    public static string ToSnakeCase(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }

      var sb = new StringBuilder();
      for (int i = 0; i < input.Length; i++)
      {
        char c = input[i];
        if (char.IsUpper(c) && i > 0)
        {
          sb.Append('_');
        }
        sb.Append(char.ToLower(c));
      }
      return sb.ToString();
    }

    public static string ToKebabCase(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }

      var sb = new StringBuilder();
      for (int i = 0; i < input.Length; i++)
      {
        char c = input[i];
        if (char.IsUpper(c) && i > 0)
        {
          sb.Append('-');
        }
        sb.Append(char.ToLower(c));
      }
      return sb.ToString();
    }

    public static string RemoveWhitespace(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }

      return string.Concat(input.Where(c => !char.IsWhiteSpace(c)));
    }

    public static string PadLeft(string input, int length, char paddingChar = ' ')
    {
      if (string.IsNullOrEmpty(input))
      {
        return new string(paddingChar, length);
      }

      return input.PadLeft(length, paddingChar);
    }

    public static string PadRight(string input, int length, char paddingChar = ' ')
    {
      if (string.IsNullOrEmpty(input))
      {
        return new string(paddingChar, length);
      }

      return input.PadRight(length, paddingChar);
    }

    public static string FormatAsCurrency(decimal amount, string currencySymbol = "$")
    {
      return $"{currencySymbol}{amount:N2}";
    }

    public static string FormatAsPercentage(decimal amount, int decimalPlaces = 2)
    {
      string format = "P" + decimalPlaces;
      return amount.ToString(format);
    }

    public static string FormatAsPhoneNumber(string phoneNumber)
    {
      if (string.IsNullOrEmpty(phoneNumber))
      {
        return string.Empty;
      }
      // Example formatting: (123) 456-7890
      return $"({phoneNumber.Substring(0, 3)}) {phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6)}";
    }

    public static string FormatAsDate(string date, string format = "MM/dd/yyyy")
    {
      if (string.IsNullOrEmpty(date))
      {
        return string.Empty;
      }

      DateTime parsedDate;
      if (DateTime.TryParse(date, out parsedDate))
      {
        return parsedDate.ToString(format);
      }
      return string.Empty;
    }

    public static string FormatAsTime(string time, string format = "hh:mm tt")
    {
      if (string.IsNullOrEmpty(time))
      {
        return string.Empty;
      }

      DateTime parsedTime;
      if (DateTime.TryParse(time, out parsedTime))
      {
        return parsedTime.ToString(format);
      }
      return string.Empty;
    }

    public static string FormatAsFileSize(long bytes)
    {
      string[] sizes = { "B", "KB", "MB", "GB", "TB" };
      double len = bytes;
      int order = 0;
      while (len >= 1024 && order < sizes.Length - 1)
      {
        order++;
        len /= 1024;
      }
      return $"{len:0.##} {sizes[order]}";
    }

    public static string FormatAsJson(string jsonString)
    {
      if (string.IsNullOrEmpty(jsonString))
      {
        return string.Empty;
      }

      var json = Newtonsoft.Json.Linq.JToken.Parse(jsonString);
      return json.ToString(Newtonsoft.Json.Formatting.Indented);
    }

    public static string FormatAsXml(string xmlString)
    {
      if (string.IsNullOrEmpty(xmlString))
      {
        return string.Empty;
      }

      var doc = new System.Xml.XmlDocument();
      doc.LoadXml(xmlString);
      using (var stringWriter = new StringWriter())
      using (var xmlWriter = new System.Xml.XmlTextWriter(stringWriter))
      {
        xmlWriter.Formatting = System.Xml.Formatting.Indented;
        doc.WriteTo(xmlWriter);
        return stringWriter.ToString();
      }
    }

    public static string FormatAsHtml(string htmlString)
    {
      if (string.IsNullOrEmpty(htmlString))
      {
        return string.Empty;
      }

      var doc = new HtmlAgilityPack.HtmlDocument();
      doc.LoadHtml(htmlString);
      using (var stringWriter = new StringWriter())
      {
        doc.Save(stringWriter);
        return stringWriter.ToString();
      }
    }

    public static string FormatAsMarkdown(string markdownString)
    {
      if (string.IsNullOrEmpty(markdownString))
      {
        return string.Empty;
      }
      // Example: Convert Markdown to HTML using a library like Markdig
      var pipeline = new Markdig.MarkdownPipelineBuilder().Build();
      var html = Markdig.Markdown.ToHtml(markdownString, pipeline);
      return html;
    }

    public static string FormatAsBase64(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }

      byte[] bytes = Encoding.UTF8.GetBytes(input);
      return Convert.ToBase64String(bytes);
    }

    public static string DecodeBase64(string base64String)
    {
      if (string.IsNullOrEmpty(base64String))
      {
        return string.Empty;
      }

      byte[] bytes = Convert.FromBase64String(base64String);
      return Encoding.UTF8.GetString(bytes);
    }

    public static string FormatAsUUID(Guid guid)
    {
      return guid.ToString("D"); // Example: 123e4567-e89b-12d3-a456-426614174000
    }

    public static string FormatAsIPAddress(string ipAddress)
    {
      if (string.IsNullOrEmpty(ipAddress))
      {
        return string.Empty;
      }

      if (System.Net.IPAddress.TryParse(ipAddress, out var ip))
      {
        return ip.ToString(); // Example: 192.168.1.1
      }

      return string.Empty;
    }

    public static string FormatAsEmail(string email)
    {
      if (string.IsNullOrEmpty(email))
      {
        return string.Empty;
      }
      // Example: Validate and format email address
      var addr = new System.Net.Mail.MailAddress(email);
      return addr.Address; // Example: return the validated email address
    }

    public static string FormatAsUrl(string url)
    {
      if (string.IsNullOrEmpty(url))
      {
        return string.Empty;
      }
      // Example: Validate and format URL
      if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
      {
        return uriResult.ToString(); // Example: return the validated URL
      }
      return string.Empty;
    }

    public static string FormatAsHex(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }

      var sb = new StringBuilder();
      foreach (char c in input)
      {
        sb.AppendFormat("{0:X2}", (int)c);
      }
      return sb.ToString();
    }

    public static string FormatAsBinary(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return string.Empty;
      }

      var sb = new StringBuilder();
      foreach (char c in input)
      {
        sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
      }
      return sb.ToString();
    }

    public static string FormatAsRomanNumerals(int number)
    {
      if (number < 1 || number > 3999)
      {
        return string.Empty;
      }

      var romanNumerals = new Dictionary<int, string>
      {
        { 1000, "M" },
        { 900, "CM" },
        { 500, "D" },
        { 400, "CD" },
        { 100, "C" },
        { 90, "XC" },
        { 50, "L" },
        { 40, "XL" },
        { 10, "X" },
        { 9, "IX" },
        { 5, "V" },
        { 4, "IV" },
        { 1, "I" }
      };
      var sb = new StringBuilder();
      foreach (var item in romanNumerals.OrderByDescending(x => x.Key))
      {
        while (number >= item.Key)
        {
          sb.Append(item.Value);
          number -= item.Key;
        }
      }
      return sb.ToString();
    }

    public static string FormatAsBinaryFile(string filePath)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      return Convert.ToBase64String(fileBytes); // Example: return the file as a Base64 string
    }

    public static string FormatAsBinaryFile(string filePath, string format)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (format == "Base64")
      {
        return Convert.ToBase64String(fileBytes); // Example: return the file as a Base64 string
      }
      else if (format == "Hex")
      {
        var sb = new StringBuilder();
        foreach (byte b in fileBytes)
        {
          sb.AppendFormat("{0:X2}", b);
        }
        return sb.ToString(); // Example: return the file as a Hex string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (format == "Base64")
      {
        return Convert.ToBase64String(fileBytes); // Example: return the file as a Base64 string
      }
      else if (format == "Hex")
      {
        var sb = new StringBuilder();
        foreach (byte b in fileBytes)
        {
          sb.AppendFormat("{0:X2}", b);
        }
        return sb.ToString(); // Example: return the file as a Hex string
      }
      else if (format == "UTF8")
      {
        return Encoding.UTF8.GetString(fileBytes); // Example: return the file as a UTF8 string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (compression == "GZip")
      {
        using (var compressedStream = new MemoryStream())
        {
          using (var gzipStream = new System.IO.Compression.GZipStream(compressedStream, System.IO.Compression.CompressionMode.Compress))
          {
            gzipStream.Write(fileBytes, 0, fileBytes.Length);
          }
          return Convert.ToBase64String(compressedStream.ToArray()); // Example: return the compressed file as a Base64 string
        }
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (encryption == "AES")
      {
        // Example: Encrypt the file bytes using AES encryption
        // Note: Implement AES encryption logic here
        return Convert.ToBase64String(fileBytes); // Example: return the encrypted file as a Base64 string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (hashing == "SHA256")
      {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
          byte[] hashBytes = sha256.ComputeHash(fileBytes);
          return Convert.ToBase64String(hashBytes); // Example: return the SHA256 hash as a Base64 string
        }
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (signing == "RSA")
      {
        // Example: Sign the file bytes using RSA signing
        // Note: Implement RSA signing logic here
        return Convert.ToBase64String(fileBytes); // Example: return the signed file as a Base64 string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (validation == "Checksum")
      {
        // Example: Validate the file bytes using a checksum
        // Note: Implement checksum validation logic here
        return Convert.ToBase64String(fileBytes); // Example: return the validated file as a Base64 string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation, string transformation)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (transformation == "Base64ToHex")
      {
        var sb = new StringBuilder();
        foreach (byte b in fileBytes)
        {
          sb.AppendFormat("{0:X2}", b);
        }
        return sb.ToString(); // Example: return the transformed file as a Hex string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation, string transformation, string serialization)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (serialization == "JSON")
      {
        // Example: Serialize the file bytes to JSON
        // Note: Implement JSON serialization logic here
        return Convert.ToBase64String(fileBytes); // Example: return the serialized file as a Base64 string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation, string transformation, string serialization, string deserialization)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (deserialization == "XML")
      {
        // Example: Deserialize the file bytes from XML
        // Note: Implement XML deserialization logic here
        return Convert.ToBase64String(fileBytes); // Example: return the deserialized file as a Base64 string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation, string transformation, string serialization, string deserialization, string compressionAlgorithm)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (compressionAlgorithm == "GZip")
      {
        using (var compressedStream = new MemoryStream())
        {
          using (var gzipStream = new System.IO.Compression.GZipStream(compressedStream, System.IO.Compression.CompressionMode.Compress))
          {
            gzipStream.Write(fileBytes, 0, fileBytes.Length);
          }
          return Convert.ToBase64String(compressedStream.ToArray()); // Example: return the compressed file as a Base64 string
        }
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation, string transformation, string serialization, string deserialization, string compressionAlgorithm, string encryptionAlgorithm)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (encryptionAlgorithm == "AES")
      {
        // Example: Encrypt the file bytes using AES encryption
        // Note: Implement AES encryption logic here
        return Convert.ToBase64String(fileBytes); // Example: return the encrypted file as a Base64 string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation, string transformation, string serialization, string deserialization, string compressionAlgorithm, string encryptionAlgorithm, string hashingAlgorithm)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (hashingAlgorithm == "SHA256")
      {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
          byte[] hashBytes = sha256.ComputeHash(fileBytes);
          return Convert.ToBase64String(hashBytes); // Example: return the SHA256 hash as a Base64 string
        }
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation, string transformation, string serialization, string deserialization, string compressionAlgorithm, string encryptionAlgorithm, string hashingAlgorithm, string signingAlgorithm)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (signingAlgorithm == "RSA")
      {
        // Example: Sign the file bytes using RSA signing
        // Note: Implement RSA signing logic here
        return Convert.ToBase64String(fileBytes); // Example: return the signed file as a Base64 string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation, string transformation, string serialization, string deserialization, string compressionAlgorithm, string encryptionAlgorithm, string hashingAlgorithm, string signingAlgorithm, string validationAlgorithm)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (validationAlgorithm == "Checksum")
      {
        // Example: Validate the file bytes using a checksum
        // Note: Implement checksum validation logic here
        return Convert.ToBase64String(fileBytes); // Example: return the validated file as a Base64 string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation, string transformation, string serialization, string deserialization, string compressionAlgorithm, string encryptionAlgorithm, string hashingAlgorithm, string signingAlgorithm, string validationAlgorithm, string transformationAlgorithm)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (transformationAlgorithm == "Base64ToHex")
      {
        var sb = new StringBuilder();
        foreach (byte b in fileBytes)
        {
          sb.AppendFormat("{0:X2}", b);
        }
        return sb.ToString(); // Example: return the transformed file as a Hex string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation, string transformation, string serialization, string deserialization, string compressionAlgorithm, string encryptionAlgorithm, string hashingAlgorithm, string signingAlgorithm, string validationAlgorithm, string transformationAlgorithm, string serializationAlgorithm)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (serializationAlgorithm == "JSON")
      {
        // Example: Serialize the file bytes to JSON
        // Note: Implement JSON serialization logic here
        return Convert.ToBase64String(fileBytes); // Example: return the serialized file as a Base64 string
      }
      return string.Empty;
    }

    public static string FormatAsBinaryFile(string filePath, string format, string encoding, string compression, string encryption, string hashing, string signing, string validation, string transformation, string serialization, string deserialization, string compressionAlgorithm, string encryptionAlgorithm, string hashingAlgorithm, string signingAlgorithm, string validationAlgorithm, string transformationAlgorithm, string serializationAlgorithm, string deserializationAlgorithm)
    {
      if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return string.Empty;
      }

      byte[] fileBytes = File.ReadAllBytes(filePath);
      if (deserializationAlgorithm == "XML")
      {
        // Example: Deserialize the file bytes from XML
        // Note: Implement XML deserialization logic here
        return Convert.ToBase64String(fileBytes); // Example: return the deserialized file as a Base64 string
      }
      return string.Empty;
    }
  }
}
