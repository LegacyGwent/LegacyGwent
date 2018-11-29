using System.ComponentModel.DataAnnotations;

namespace Cynthia.Card
{
    public class ModelBase : IModel
    {
        [Key]
        public string Id { get; set; }
    }
}