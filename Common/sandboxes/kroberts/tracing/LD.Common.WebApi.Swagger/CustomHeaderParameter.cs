using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace LD.Common.WebApi.Swagger
{
    /// <summary>
    /// Operation filter for Swagger UI to add a custom header parameter
    /// </summary>
    public class CustomHeaderParameter : IOperationFilter
    {
        /// <summary>
        /// Function which will be called with each api description
        /// Return true to add the header to the swagger operation
        /// </summary>
        public Func<ApiDescription, bool> Filter { get; set; }

        /// <summary>User friendly description of the header</summary>
        public string Description { get; set; }

        /// <summary>A unique identifier for the header (auto set to guid)</summary>
        public string Key { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Name of the header (e.g. Authorization)</summary>
        public string Name { get; set; }

        /// <summary>Default pre-filled value for the header</summary>
        public string Default { get; set; }

        /// <summary>
        /// Apply header config to a full configuration
        /// </summary>
        /// <param name="c"></param>
        public void Apply(SwaggerDocsConfig c)
        {
            c.ApiKey(Key).Name(Name).Description(Description).In("header");
            c.OperationFilter(() => this);
        }

        /// <summary>
        /// Apply header config to a specific operation
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (!(Filter?.Invoke(apiDescription) ?? false))
                return;

            operation.parameters = operation.parameters ?? new List<Parameter>();
            operation.parameters.Add(new Parameter
            {
                name = Name,
                description = Description,
                @in = "header",
                required = true,
                type = "string",
                @default = Default
            });
        }
    }
}
