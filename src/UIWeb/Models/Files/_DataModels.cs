using System.ComponentModel.DataAnnotations;

namespace Wiz.Gringotts.UIWeb.Models.Files
{
    public enum FileType
    {
        Image = 1
    }
    public class File
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public FileType FileType { get; set; }
    }
}