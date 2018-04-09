using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class GetAuditLogsParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<ulong> BeforeEntryId { get; set; }
        public Optional<ulong> UserId { get; set; }
        public Optional<ActionType> ActionType { get; set; }
    }
}
