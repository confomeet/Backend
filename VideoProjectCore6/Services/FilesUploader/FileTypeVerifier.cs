namespace VideoProjectCore6.Services.FilesUploader
{
    public static class FileTypeVerifier
    {
      /*  private static FileTypeVerifyResult Unknown = new FileTypeVerifyResult
        {
            Name = "Unknown",
            Description = "Unknown File Type",
            IsVerified = false
        };

        static FileTypeVerifier()
        {
            Types = new List<FileType>
                {
                    new Jpeg(),
                    new Png(),
                    new Pdf()

                }
                .OrderByDescending(x => x.SignatureLength)
                .ToList();
        }

        private static IEnumerable<FileType> Types { get; set; }

        public static FileTypeVerifyResult CheckFile(string path)
        {
            using var file = File.OpenRead(path);
            FileTypeVerifyResult result = null;

            foreach (var fileType in Types)
            {
                result = fileType.Verify(file);
                if (result.IsVerified)
                    break;
            }

            return result?.IsVerified == true
                   ? result
                   : Unknown;
        }



        public static FileTypeVerifyResult CheckStream(Stream stream)
        {

            //using var file = File.OpenRead(path);
            FileTypeVerifyResult result = null;

            foreach (var fileType in Types)
            {
                result = fileType.Verify(stream);
                if (result.IsVerified)
                    break;
            }

            return result?.IsVerified == true
                   ? result
                   : Unknown;
        }*/
    }
}
