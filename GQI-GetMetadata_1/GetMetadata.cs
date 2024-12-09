namespace GQIGetMetadata
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Net.Helper;

    [GQIMetaData(Name = "Get Metadata")]
    public class GetMetadata : IGQIDataSource, IGQIInputArguments
    {
        private readonly GQIStringArgument metadataJsonArg = new GQIStringArgument("Json Metadata") { IsRequired = false };
        private Dictionary<string, object> metadataDictionary;

        public GQIArgument[] GetInputArguments()
        {
            return new GQIArgument[] { metadataJsonArg };
        }

        public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
        {
            try
            {
                var metadataJson = string.IsNullOrWhiteSpace(args.GetArgumentValue(metadataJsonArg)) ? string.Empty : args.GetArgumentValue(metadataJsonArg);

                metadataDictionary = metadataJson.IsNullOrEmpty() ? new Dictionary<string, object>() : JsonConvert.DeserializeObject<Dictionary<string, object>>(metadataJson);

                return default;
            }
            catch (Exception ex)
            {
                throw new Exception("Please, select one row so valid data could be shown.");
            }
        }

        public GQIColumn[] GetColumns()
        {
            return new GQIColumn[]
            {
                new GQIStringColumn("Key"),
                new GQIStringColumn("Value"),
            };
        }

        public GQIPage GetNextPage(GetNextPageInputArgs args)
        {
            var rows = new List<GQIRow>();

            foreach (var kvp in metadataDictionary)
            {
                rows.Add(CreateRow(kvp.Key, kvp.Value));
            }

            return new GQIPage(rows.ToArray());
        }

        private GQIRow CreateRow(string key, object value)
        {
            return new GQIRow(
                new[]
                {
                    new GQICell
                    {
                        Value = key,
                    },
                    new GQICell
                    {
                        Value = value != null ? value.ToString() : string.Empty,
                    },
                });
        }
    }
}
