using FSL.Benchmark.AspNetFramework.Models;
using FSL.Benchmark.AspNetFramework.Repository;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FSL.Benchmark.AspNetFramework.Controllers
{
    [RoutePrefix("api/benchmark")]
    public class BenchmarkController : ApiController
    {
        private readonly AddressSqlRepository _addressRepository;

        public BenchmarkController()
        {
            _addressRepository = new AddressSqlRepository();
        }

        [Route("range")]
        public async Task<IEnumerable<Address>> GetRangeAsync(
            string start,
            string end)
        {
            return await _addressRepository.GetAddressRangeAsync(
                start,
                end);
        }
        
        [Route("range/txt")]
        public async Task<string> GetRangeTxtAsync(
            string start,
            string end)
        {
            var addresses = await _addressRepository.GetAddressRangeAsync(
                start,
                end);

            string txt;

            if (addresses == null || addresses.Count() == 0)
            {
                txt = "addresses NULL or EMPTY";
            }
            else
            {
                var sb = new StringBuilder();

                sb.Append(GetColumns<Address>());
                sb.Append("\r\n");

                foreach (var address in addresses)
                {
                    sb.Append(GetColumns(address));
                    sb.Append("\r\n");
                }

                txt = sb.ToString();
            }

            return txt;
        }

        [Route("filesystem")]
        public IEnumerable<string> GetFileSystem()
        {
            var files = Directory.GetFiles(@"c:\windows");

            return files;
        }

        [Route("filesystem/download")]
        public HttpResponseMessage GetFileSystemDownload()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Cartilha_do_Idoso.pdf");

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read))
            };

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Path.GetFileName(path)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            return response;
        }

        [Route("{id}")]
        public async Task<Address> GetAsync(
            string id)
        {
            return await _addressRepository.GetAddressAsync(id);
        }

        private string GetColumns<T>(
            T data = default)
        {
            var sb = new StringBuilder();
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (sb.Length > 0)
                {
                    sb.Append(";");
                }

                if (data == null)
                {
                    sb.AppendFormat(
                        "{0}_{1}",
                        type.Name,
                        property.Name);
                }
                else
                {
                    var val = property.GetValue(data);

                    sb.Append(val == null ? "" : $" {val.ToString()}");
                }
            }

            sb.Append(";");

            return sb.ToString();
        }
    }
}