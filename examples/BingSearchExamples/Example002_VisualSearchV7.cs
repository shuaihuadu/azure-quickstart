namespace BingSearchExamples;

public class Example001_VisualSearchV7(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public void VisionSearch_ImageBinary()
    {
        VisualSearchV7.Run(this.Output);
    }
}

internal class VisualSearchV7
{
    // Add your Azure Bing Search V7 subscription key and endpoint to your environment variables.
    static string subscriptionKey = TestConfiguration.Bing.ApiKey;
    static string endpoint = "https://api.bing.microsoft.com/v7.0/images/visualsearch";

    // Set the path to the image that you want to get insights of. 
    static string imagePath = @"Resources/2.jpg";

    // Boundary strings for form data in body of POST.
    const string CRLF = "\r\n";
    static string BoundaryTemplate = "batch_{0}";
    static string StartBoundaryTemplate = "--{0}";
    static string EndBoundaryTemplate = "--{0}--";

    const string CONTENT_TYPE_HEADER_PARAMS = "multipart/form-data; boundary={0}";
    const string POST_BODY_DISPOSITION_HEADER = "Content-Disposition: form-data; name=\"image\"; filename=\"{0}\"" + CRLF + CRLF;


    internal static void Run(ITestOutputHelper output)
    {
        try
        {
            // Gets image.
            var filename = new FileInfo(imagePath).FullName;
            output.WriteLine("Getting image insights for image: " + Path.GetFileName(filename));
            var imageBinary = File.ReadAllBytes(imagePath);

            // Sets up POST body.
            var boundary = string.Format(BoundaryTemplate, Guid.NewGuid());

            // Builds form start data.
            var startBoundary = string.Format(StartBoundaryTemplate, boundary);
            var startFormData = startBoundary + CRLF;
            startFormData += string.Format(POST_BODY_DISPOSITION_HEADER, filename);

            // Builds form end data.
            var endFormData = CRLF + CRLF + string.Format(EndBoundaryTemplate, boundary) + CRLF;
            var contentTypeHeaderValue = string.Format(CONTENT_TYPE_HEADER_PARAMS, boundary);

            // Sets up the request for a visual search.
            WebRequest request = HttpWebRequest.Create(endpoint);
            request.ContentType = contentTypeHeaderValue;
            request.Headers["Ocp-Apim-Subscription-Key"] = subscriptionKey;
            request.Method = "POST";

            // Writes the boundary and Content-Disposition header, then writes
            // the image binary, and finishes by writing the closing boundary.
            using (Stream requestStream = request.GetRequestStream())
            {
                StreamWriter writer = new StreamWriter(requestStream);
                writer.Write(startFormData);
                writer.Flush();
                requestStream.Write(imageBinary, 0, imageBinary.Length);
                writer.Write(endFormData);
                writer.Flush();
                writer.Close();
            }

            // Calls the Bing Visual Search endpoint and returns the JSON response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            output.WriteLine("\nJSON Response:\n");
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            output.WriteLine(JsonConvert.SerializeObject(parsedJson, Formatting.Indented));
        }
        catch (Exception e)
        {
            output.WriteLine(e.Message);
        }
    }
}

