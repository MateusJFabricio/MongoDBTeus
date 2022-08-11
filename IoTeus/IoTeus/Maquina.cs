using MongoDB.Bson;

namespace IoTeus
{
    public class Maquina
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Localizacao { get; set; }
        public string Tipo { get; set; }
        public string DiasOperacao { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
