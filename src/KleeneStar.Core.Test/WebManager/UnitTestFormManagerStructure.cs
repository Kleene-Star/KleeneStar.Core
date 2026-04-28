using KleeneStar.Core.Test;
using KleeneStar.Model.Entities;
using KleeneStar.Model.Forms;
using Microsoft.EntityFrameworkCore;

namespace KleeneStar.Core.Test.WebManager
{
    /// <summary>
    /// Provides unit tests for the structure-related methods of <see cref="KleeneStar.Core.WebManager.FormManager"/>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestFormManagerStructure
    {
        private static readonly Guid WorkspaceId = Guid.Parse("3946B811-DFBB-4575-A83B-5C1C0240DF22");
        private static readonly Guid ClassId = Guid.Parse("B54AA5B2-01D5-490A-90A3-4D57FE50320B");

        private static (Guid FormId, Guid FieldId) Seed(string connectionString)
        {
            CoreHubFixture.Initialize(connectionString);

            var formId = Guid.NewGuid();
            var fieldId = Guid.NewGuid();

            using var db = CoreHubFixture.CreateDbContext(connectionString);

            db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-1", Name = "workspace" });
            db.Classes.Add(new Class { Id = ClassId, Name = "class", WorkspaceId = WorkspaceId });
            db.Forms.Add(new Form { Id = formId, Name = "Custom", FormType = FormType.Default, ClassId = ClassId });
            db.Fields.Add(new Field
            {
                Id = fieldId,
                Name = "Title",
                FieldType = FieldType.Text,
                ClassId = ClassId,
                State = FieldState.Active
            });
            db.SaveChanges();

            return (formId, fieldId);
        }

        [Fact]
        public void Save_Then_Retrieve_RoundTrip()
        {
            // arrange
            var (formId, fieldId) = Seed(nameof(Save_Then_Retrieve_RoundTrip));

            var snapshot = new FormStructureSnapshot
            {
                FormName = "Custom",
                FormDescription = "Test",
                Tabs =
                [
                    new TabSnapshot
                    {
                        Name = "Main",
                        Children =
                        [
                            new GroupSnapshot
                            {
                                Label = "Section",
                                Layout = FormGroupLayout.Vertical,
                                Children = [new FieldRefSnapshot { FieldId = fieldId }]
                            }
                        ]
                    }
                ]
            };

            // act
            var newVersion = CoreHub.FormManager.SaveFormStructure(formId, snapshot, expectedVersion: 0);
            var loaded = CoreHub.FormManager.GetFormWithStructure(formId);

            // validation
            Assert.Equal(1, newVersion);
            Assert.NotNull(loaded);
            Assert.Equal(1, loaded.Version);
            var tab = Assert.Single(loaded.Tabs);
            var group = Assert.IsType<FormGroupElement>(Assert.Single(tab.Elements));
            var fieldRef = Assert.IsType<FormFieldRefElement>(Assert.Single(group.Children));
            Assert.Equal(fieldId, fieldRef.FieldId);
        }

        [Fact]
        public void Save_StaleVersion_Throws()
        {
            // arrange
            var (formId, _) = Seed(nameof(Save_StaleVersion_Throws));

            CoreHub.FormManager.SaveFormStructure(formId, new FormStructureSnapshot { FormName = "v1" }, expectedVersion: 0);

            // act / validation
            Assert.Throws<DbUpdateConcurrencyException>(() =>
                CoreHub.FormManager.SaveFormStructure(formId, new FormStructureSnapshot { FormName = "v2" }, expectedVersion: 0));
        }

        [Fact]
        public void GetFormWithStructure_ReturnsNullForUnknownId()
        {
            // arrange
            CoreHubFixture.Initialize(nameof(GetFormWithStructure_ReturnsNullForUnknownId));

            // act
            var result = CoreHub.FormManager.GetFormWithStructure(Guid.NewGuid());

            // validation
            Assert.Null(result);
        }
    }
}
