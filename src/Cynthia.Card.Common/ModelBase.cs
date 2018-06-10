using System.ComponentModel.DataAnnotations;

namespace Cynthia.Card.Common
{
    public class ModelBase : IModel
    {
        [Key]
        public string Id { get; set; }
    }
}