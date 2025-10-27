namespace Plantonize.NotasFiscais.Infrastructure.Configuration
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string NotasFiscaisCollectionName { get; set; } = string.Empty;
        public string FaturasCollectionName { get; set; } = string.Empty;
        public string MunicipiosAliquotaCollectionName { get; set; } = string.Empty;
        public string ImpostosResumoCollectionName { get; set; } = string.Empty;
    }
}
