using FluentMigrator;
using System.Data;

namespace ArticleDBLib
{
    //migracja tabel do bazy, wstawi też jedną dodatkową informującą o wersji migracji oraz dacie,
    //kiedy nastąpiła ta migracja
    //
    [Migration(0)]
    public class DbMigration : Migration
    {
        public override void Up()
        {
            Create.Table("articles")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("title").AsCustom("varchar(100)").NotNullable().Unique()
                .WithColumn("journal").AsCustom("varchar(50)").NotNullable()
                .WithColumn("year").AsInt32()
                .WithColumn("volume").AsInt32()
                .WithColumn("number").AsInt32()
                .WithColumn("pages").AsInt32();
            Create.Table("authors")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("author").AsCustom("varchar(20)").NotNullable().Unique();
            Create.Table("keywords")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("keyword").AsCustom("varchar(15)").NotNullable().Unique();
            Create.Table("files")
                .WithColumn("id_article").AsInt32().NotNullable().ForeignKey("fk_id_fil","articles","id").OnDelete(Rule.Cascade)
                .WithColumn("filename").AsString().NotNullable();
            Create.Table("comments")
                .WithColumn("id").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("id_article").AsInt32().NotNullable().ForeignKey("fk_id_com","articles","id").OnDelete(Rule.Cascade)
                .WithColumn("commenter").AsCustom("varchar(20)").NotNullable()
                .WithColumn("comment").AsCustom("varchar(250)").NotNullable();
            Create.Table("authorsofarticles")
                .WithColumn("id_article").AsInt32().NotNullable().ForeignKey("aofa_art","articles","id").OnDelete(Rule.Cascade)
                .WithColumn("id_author").AsInt32().NotNullable().ForeignKey("aifa_aut","authors","id").OnDelete(Rule.Cascade);
            Create.Table("keywordstoarticles")
                .WithColumn("id_article").AsInt32().NotNullable().ForeignKey("ktoa_art","articles","id").OnDelete(Rule.Cascade)
                .WithColumn("id_keyword").AsInt32().NotNullable().ForeignKey("ktoa_kwrds","keywords","id").OnDelete(Rule.Cascade);
        }
        public override void Down()
        {
            //CELOWO ZOSTAWIONE PUSTE, BO NIE BĘDZIE USUWANAZ BAZY ŻADNA TABLICA. TYLKO BĘDĄ DODAWANE.
            //Metoda Down() zawsze musi zostać zawarta.
        }
    }
    //Potrzebne, bo bez tego nie ruszy migracja
    //Zapisze, co się stało w razie, gdyby coś poszło w trakcie migracji nie tak.
    //Nie powinno się to zdarzyć, jednak musi zostać to zawarte.
    public class AddLogTable : Migration
    {
        public override void Up()
        {
            Create.Table("Log")
                .WithColumn("id").AsInt32().PrimaryKey().NotNullable()
                .WithColumn("text").AsString();
        }
        public override void Down()
        {
            Delete.Table("Log");
        }
    }
}
