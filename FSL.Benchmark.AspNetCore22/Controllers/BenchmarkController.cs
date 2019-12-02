using FSL.Benchmark.AspNetCore22.Models;
using FSL.Benchmark.AspNetCore22.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FSL.Benchmark.AspNetCore22.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class BenchmarkController : ControllerBase
    {
        private readonly AddressSqlRepository _addressRepository;
        private readonly IHostingEnvironment _env;

        public BenchmarkController(
            IConfiguration configuration,
            IHostingEnvironment env)
        {
            _addressRepository = new AddressSqlRepository(configuration);
            _env = env;
        } 

        [HttpGet("range")]
        public async Task<IEnumerable<Address>> GetRangeAsync(
            string start,
            string end)
        {
            return await _addressRepository.GetAddressRangeAsync(
                start,
                end);
        }

        [HttpGet("range/txt")]
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

        [HttpGet("filesystem")]
        public IEnumerable<string> GetFileSystem()
        {
            var files = Directory.GetFiles(@"c:\windows");

            return files;
        }


        [HttpGet("filesystem/download")]
        public IActionResult GetFileSystemDownload()
        {
            var path = $@"{_env.ContentRootPath}\App_Data\Cartilha_do_Idoso.pdf";

            return new FileStreamResult(new FileStream(path, FileMode.Open, FileAccess.Read), "application/pdf");
        }

        [HttpGet("{id}")]
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
