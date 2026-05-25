using KleeneStar.Core.Test;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Core.Test.WebManager
{
    /// <summary>
    /// Provides unit tests for <see cref="KleeneStar.Core.WebManager.IdentityManager"/>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestIdentityManager
    {
        /// <summary>
        /// Initializes the in-memory database and CoreHub for a single test case.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        private static void Seed(string connectionString)
        {
            CoreHubFixture.Initialize(connectionString);
        }

        /// <summary>
        /// Verifies that <c>Add</c> persists the identity and that <c>GetIdentity</c>
        /// retrieves it by its business id.
        /// </summary>
        [Fact]
        public void Add_Then_GetIdentity_RoundTrip()
        {
            Seed(nameof(Add_Then_GetIdentity_RoundTrip));

            var identity = Sample("Alice", "alice@kleenestar.test");
            CoreHub.IdentityManager.Add(identity);

            var loaded = CoreHub.IdentityManager.GetIdentity(identity.Id);

            Assert.NotNull(loaded);
            Assert.Equal("Alice", loaded.Name);
            Assert.Equal("alice@kleenestar.test", loaded.Email);
        }

        /// <summary>
        /// Verifies that <c>Update</c> writes scalar property changes back to the database.
        /// </summary>
        [Fact]
        public void Update_ChangesScalars()
        {
            Seed(nameof(Update_ChangesScalars));

            var identity = Sample("Alice", "alice@kleenestar.test");
            CoreHub.IdentityManager.Add(identity);

            identity.Name = "Alice Cooper";
            CoreHub.IdentityManager.Update(identity);

            var loaded = CoreHub.IdentityManager.GetIdentity(identity.Id);
            Assert.NotNull(loaded);
            Assert.Equal("Alice Cooper", loaded.Name);
        }

        /// <summary>
        /// Verifies that <c>Remove</c> deletes the identity and raises the
        /// <see cref="KleeneStar.Core.WebManager.IIdentityManager.IdentityRemoved"/> event.
        /// </summary>
        [Fact]
        public void Remove_DeletesAndRaisesEvent()
        {
            Seed(nameof(Remove_DeletesAndRaisesEvent));

            var identity = Sample("DeleteMe", "del@kleenestar.test");
            CoreHub.IdentityManager.Add(identity);

            Identity raised = null;
            CoreHub.IdentityManager.IdentityRemoved += (_, i) => raised = i;

            CoreHub.IdentityManager.Remove(identity.Id);

            Assert.Null(CoreHub.IdentityManager.GetIdentity(identity.Id));
            Assert.NotNull(raised);
            Assert.Equal(identity.Id, raised.Id);
        }

        /// <summary>
        /// Verifies that <c>GetIdentities(IQuery)</c> returns identities from the database.
        /// </summary>
        [Fact]
        public void GetIdentities_ReturnsAllStored()
        {
            Seed(nameof(GetIdentities_ReturnsAllStored));

            CoreHub.IdentityManager.Add(Sample("Alpha", "a@x"));
            CoreHub.IdentityManager.Add(Sample("Beta", "b@x"));

            var result = CoreHub.IdentityManager.GetIdentities(new Query<Identity>()).ToList();

            Assert.True(result.Count >= 2);
            Assert.Contains(result, i => i.Name == "Alpha");
            Assert.Contains(result, i => i.Name == "Beta");
        }

        /// <summary>
        /// Verifies that <c>ReservedIdentityNames</c> blocks well-known URL segments
        /// that would otherwise collide with router endpoints.
        /// </summary>
        [Fact]
        public void ReservedIdentityNames_BlocksRouterSegments()
        {
            Assert.Contains("admin",  KleeneStar.Core.WebManager.IdentityManager.ReservedIdentityNames);
            Assert.Contains("system", KleeneStar.Core.WebManager.IdentityManager.ReservedIdentityNames);
            Assert.Contains("api",    KleeneStar.Core.WebManager.IdentityManager.ReservedIdentityNames);
        }

        /// <summary>
        /// Creates a sample <see cref="Identity"/> with a fresh GUID.
        /// </summary>
        /// <param name="name">The display name.</param>
        /// <param name="email">The email address.</param>
        /// <returns>The sample identity.</returns>
        private static Identity Sample(string name, string email) => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = "$seed$v1$test",
            State = IdentityState.Active
        };
    }
}
