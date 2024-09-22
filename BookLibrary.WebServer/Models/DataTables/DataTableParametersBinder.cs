using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLibrary.WebServer.Models.DataTables
{
    public class DataTableParametersBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var query = bindingContext.HttpContext.Request.Query;
            var columns = new List<Column>();
            var orders = new List<Order>();

            for (int i = 0; ; i++)
            {
                var dataKey = $"columns[{i}][data]";
                var nameKey = $"columns[{i}][name]";
                var searchableKey = $"columns[{i}][searchable]";
                var orderableKey = $"columns[{i}][orderable]";
                var searchValueKey = $"columns[{i}][search][value]";
                var searchRegexKey = $"columns[{i}][search][regex]";

                if (!query.ContainsKey(dataKey))
                    break;

                columns.Add(new Column
                {
                    Data = int.Parse(query[dataKey]),
                    Name = query[nameKey],
                    Searchable = bool.Parse(query[searchableKey]),
                    Orderable = bool.Parse(query[orderableKey]),
                    Search = new Search
                    {
                        Value = query[searchValueKey],
                        Regex = bool.Parse(query[searchRegexKey])
                    }
                });
            }

            for (int i = 0; ; i++)
            {
                var columnKey = $"order[{i}][column]";
                var dirKey = $"order[{i}][dir]";
                var nameKey = $"order[{i}][name]";

                if (!query.ContainsKey(columnKey))
                    break;

                orders.Add(new Order
                {
                    Column = int.Parse(query[columnKey]),
                    Dir = query[dirKey],
                    Name = query[nameKey]
                });
            }

            var result = new DataTableParameters
            {
                Draw = int.Parse(query["draw"]),
                Columns = columns,
                Order = orders,
                Start = int.Parse(query["start"]),
                Length = int.Parse(query["length"]),
                Search = new Search
                {
                    Value = query["search[value]"],
                    Regex = bool.Parse(query["search[regex]"])
                }
            };

            bindingContext.Result = ModelBindingResult.Success(result);
            return Task.CompletedTask;
        }
    }
}
