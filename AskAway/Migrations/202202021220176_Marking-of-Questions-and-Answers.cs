namespace AskAway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MarkingofQuestionsandAnswers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Replies", "Topic_Id", "dbo.Topics");
            DropIndex("dbo.Replies", new[] { "Topic_Id" });
            AddColumn("dbo.Topics", "ClosedTopic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Replies", "CorrectAnswer", c => c.Boolean(nullable: false));
            AddColumn("dbo.Replies", "TopicId", c => c.Int(nullable: false));
            DropColumn("dbo.Replies", "Topic_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Replies", "Topic_Id", c => c.Int());
            DropColumn("dbo.Replies", "TopicId");
            DropColumn("dbo.Replies", "CorrectAnswer");
            DropColumn("dbo.Topics", "ClosedTopic");
            CreateIndex("dbo.Replies", "Topic_Id");
            AddForeignKey("dbo.Replies", "Topic_Id", "dbo.Topics", "Id");
        }
    }
}
