using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Core.Models
{
    public class FileModel
    {
        public byte[] Content { get; }
        public string FileName { get; }
        public string ContentType { get; }

        public FileModel(byte[] content, string fileName, string contentType)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        }
    }
}
