using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jone_fora
{
    public class Data
    {
        [JsonProperty("Data")]
        public DateOnly DataAtual { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public int QuantidadeSaidas { get; set; } = 0;
        public TimeSpan QuantidadeMinutos { get; set; } = TimeSpan.Zero;

    }
}
