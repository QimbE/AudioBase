using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeDBLinkageLogicAndTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoAuthor_Artist_CoAuthorId",
                table: "CoAuthor");

            migrationBuilder.DropForeignKey(
                name: "FK_CoAuthor_Track_TrackId",
                table: "CoAuthor");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelRelease_Label_LabelId",
                table: "LabelRelease");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelRelease_Release_ReleaseId",
                table: "LabelRelease");

            migrationBuilder.DropForeignKey(
                name: "FK_Release_Artist_AuthorId",
                table: "Release");

            migrationBuilder.DropForeignKey(
                name: "FK_Release_ReleaseType_ReleaseTypeId",
                table: "Release");

            migrationBuilder.DropForeignKey(
                name: "FK_Track_Genre_GenreId",
                table: "Track");

            migrationBuilder.DropForeignKey(
                name: "FK_Track_Release_ReleaseId",
                table: "Track");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Track",
                table: "Track");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReleaseType",
                table: "ReleaseType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Release",
                table: "Release");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabelRelease",
                table: "LabelRelease");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Label",
                table: "Label");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Genre",
                table: "Genre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoAuthor",
                table: "CoAuthor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Artist",
                table: "Artist");

            migrationBuilder.RenameTable(
                name: "Track",
                newName: "Tracks");

            migrationBuilder.RenameTable(
                name: "ReleaseType",
                newName: "ReleaseTypes");

            migrationBuilder.RenameTable(
                name: "Release",
                newName: "Releases");

            migrationBuilder.RenameTable(
                name: "LabelRelease",
                newName: "LabelReleases");

            migrationBuilder.RenameTable(
                name: "Label",
                newName: "Labels");

            migrationBuilder.RenameTable(
                name: "Genre",
                newName: "Genres");

            migrationBuilder.RenameTable(
                name: "CoAuthor",
                newName: "CoAuthors");

            migrationBuilder.RenameTable(
                name: "Artist",
                newName: "Artists");

            migrationBuilder.RenameIndex(
                name: "IX_Track_ReleaseId",
                table: "Tracks",
                newName: "IX_Tracks_ReleaseId");

            migrationBuilder.RenameIndex(
                name: "IX_Track_GenreId",
                table: "Tracks",
                newName: "IX_Tracks_GenreId");

            migrationBuilder.RenameIndex(
                name: "IX_ReleaseType_Name",
                table: "ReleaseTypes",
                newName: "IX_ReleaseTypes_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Release_ReleaseTypeId",
                table: "Releases",
                newName: "IX_Releases_ReleaseTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Release_AuthorId",
                table: "Releases",
                newName: "IX_Releases_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_LabelRelease_ReleaseId",
                table: "LabelReleases",
                newName: "IX_LabelReleases_ReleaseId");

            migrationBuilder.RenameIndex(
                name: "IX_LabelRelease_LabelId",
                table: "LabelReleases",
                newName: "IX_LabelReleases_LabelId");

            migrationBuilder.RenameIndex(
                name: "IX_Label_Name",
                table: "Labels",
                newName: "IX_Labels_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Genre_Name",
                table: "Genres",
                newName: "IX_Genres_Name");

            migrationBuilder.RenameIndex(
                name: "IX_CoAuthor_TrackId",
                table: "CoAuthors",
                newName: "IX_CoAuthors_TrackId");

            migrationBuilder.RenameIndex(
                name: "IX_CoAuthor_CoAuthorId",
                table: "CoAuthors",
                newName: "IX_CoAuthors_CoAuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Artist_Name",
                table: "Artists",
                newName: "IX_Artists_Name");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Tracks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tracks",
                table: "Tracks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReleaseTypes",
                table: "ReleaseTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Releases",
                table: "Releases",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabelReleases",
                table: "LabelReleases",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Labels",
                table: "Labels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genres",
                table: "Genres",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoAuthors",
                table: "CoAuthors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Artists",
                table: "Artists",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_AuthorId",
                table: "Tracks",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoAuthors_Artists_CoAuthorId",
                table: "CoAuthors",
                column: "CoAuthorId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoAuthors_Tracks_TrackId",
                table: "CoAuthors",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelReleases_Labels_LabelId",
                table: "LabelReleases",
                column: "LabelId",
                principalTable: "Labels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelReleases_Releases_ReleaseId",
                table: "LabelReleases",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Releases_Artists_AuthorId",
                table: "Releases",
                column: "AuthorId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Releases_ReleaseTypes_ReleaseTypeId",
                table: "Releases",
                column: "ReleaseTypeId",
                principalTable: "ReleaseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Artists_AuthorId",
                table: "Tracks",
                column: "AuthorId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Genres_GenreId",
                table: "Tracks",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Releases_ReleaseId",
                table: "Tracks",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoAuthors_Artists_CoAuthorId",
                table: "CoAuthors");

            migrationBuilder.DropForeignKey(
                name: "FK_CoAuthors_Tracks_TrackId",
                table: "CoAuthors");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelReleases_Labels_LabelId",
                table: "LabelReleases");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelReleases_Releases_ReleaseId",
                table: "LabelReleases");

            migrationBuilder.DropForeignKey(
                name: "FK_Releases_Artists_AuthorId",
                table: "Releases");

            migrationBuilder.DropForeignKey(
                name: "FK_Releases_ReleaseTypes_ReleaseTypeId",
                table: "Releases");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Artists_AuthorId",
                table: "Tracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Genres_GenreId",
                table: "Tracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Releases_ReleaseId",
                table: "Tracks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tracks",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Tracks_AuthorId",
                table: "Tracks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Releases",
                table: "Releases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReleaseTypes",
                table: "ReleaseTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Labels",
                table: "Labels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabelReleases",
                table: "LabelReleases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Genres",
                table: "Genres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoAuthors",
                table: "CoAuthors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Artists",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Tracks");

            migrationBuilder.RenameTable(
                name: "Tracks",
                newName: "Track");

            migrationBuilder.RenameTable(
                name: "Releases",
                newName: "Release");

            migrationBuilder.RenameTable(
                name: "ReleaseTypes",
                newName: "ReleaseType");

            migrationBuilder.RenameTable(
                name: "Labels",
                newName: "Label");

            migrationBuilder.RenameTable(
                name: "LabelReleases",
                newName: "LabelRelease");

            migrationBuilder.RenameTable(
                name: "Genres",
                newName: "Genre");

            migrationBuilder.RenameTable(
                name: "CoAuthors",
                newName: "CoAuthor");

            migrationBuilder.RenameTable(
                name: "Artists",
                newName: "Artist");

            migrationBuilder.RenameIndex(
                name: "IX_Tracks_ReleaseId",
                table: "Track",
                newName: "IX_Track_ReleaseId");

            migrationBuilder.RenameIndex(
                name: "IX_Tracks_GenreId",
                table: "Track",
                newName: "IX_Track_GenreId");

            migrationBuilder.RenameIndex(
                name: "IX_Releases_ReleaseTypeId",
                table: "Release",
                newName: "IX_Release_ReleaseTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Releases_AuthorId",
                table: "Release",
                newName: "IX_Release_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_ReleaseTypes_Name",
                table: "ReleaseType",
                newName: "IX_ReleaseType_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Labels_Name",
                table: "Label",
                newName: "IX_Label_Name");

            migrationBuilder.RenameIndex(
                name: "IX_LabelReleases_ReleaseId",
                table: "LabelRelease",
                newName: "IX_LabelRelease_ReleaseId");

            migrationBuilder.RenameIndex(
                name: "IX_LabelReleases_LabelId",
                table: "LabelRelease",
                newName: "IX_LabelRelease_LabelId");

            migrationBuilder.RenameIndex(
                name: "IX_Genres_Name",
                table: "Genre",
                newName: "IX_Genre_Name");

            migrationBuilder.RenameIndex(
                name: "IX_CoAuthors_TrackId",
                table: "CoAuthor",
                newName: "IX_CoAuthor_TrackId");

            migrationBuilder.RenameIndex(
                name: "IX_CoAuthors_CoAuthorId",
                table: "CoAuthor",
                newName: "IX_CoAuthor_CoAuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Artists_Name",
                table: "Artist",
                newName: "IX_Artist_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Track",
                table: "Track",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Release",
                table: "Release",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReleaseType",
                table: "ReleaseType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Label",
                table: "Label",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabelRelease",
                table: "LabelRelease",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genre",
                table: "Genre",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoAuthor",
                table: "CoAuthor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Artist",
                table: "Artist",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoAuthor_Artist_CoAuthorId",
                table: "CoAuthor",
                column: "CoAuthorId",
                principalTable: "Artist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoAuthor_Track_TrackId",
                table: "CoAuthor",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelRelease_Label_LabelId",
                table: "LabelRelease",
                column: "LabelId",
                principalTable: "Label",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelRelease_Release_ReleaseId",
                table: "LabelRelease",
                column: "ReleaseId",
                principalTable: "Release",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Release_Artist_AuthorId",
                table: "Release",
                column: "AuthorId",
                principalTable: "Artist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Release_ReleaseType_ReleaseTypeId",
                table: "Release",
                column: "ReleaseTypeId",
                principalTable: "ReleaseType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Track_Genre_GenreId",
                table: "Track",
                column: "GenreId",
                principalTable: "Genre",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Track_Release_ReleaseId",
                table: "Track",
                column: "ReleaseId",
                principalTable: "Release",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
