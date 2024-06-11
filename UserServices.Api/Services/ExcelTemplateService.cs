namespace UserServices.Api.Services
{
    public class ExcelTemplateService
    {
        private readonly IWebHostEnvironment _env;

        public ExcelTemplateService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] GetExcelTemplate(string templateName)
        {
            string templateFilePath = Path.Combine(_env.WebRootPath, "Template", templateName + ".xlsx");

            using (FileStream fileStream = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    fileStream.CopyTo(stream);
                    return stream.ToArray();
                }
            }
        }
    }

}
