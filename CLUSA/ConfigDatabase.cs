namespace CLUSA
{
    public static class ConfigDatabase
    {
        public static string MongoConnectionString =>
            "mongodb+srv://dev:dev@cluster0.cn10nzt.mongodb.net/"; //Produção
            /*"mongodb+srv://dev:dev@testeusa.kt5go1v.mongodb.net/";*/ //Desenvolvimento

        public static string MongoDatabaseName => "Trabalho";
    }
}