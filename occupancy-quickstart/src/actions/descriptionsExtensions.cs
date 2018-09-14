using System;

// These extensions translate the descriptions (which are in memory representations
// of the sample yaml file) to Digital Twins models.

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public static class DescriptionExtensions
    {
        public static Models.SpaceCreate ToSpaceCreate(this SpaceDescription description, Guid parentId)
            => new Models.SpaceCreate()
            {
                Name = description.name,
                ParentSpaceId = parentId != Guid.Empty ? parentId.ToString() : "",
            };

        public static Models.ResourceCreate ToResourceCreate(this ResourceDescription description, Guid spaceId)
            => new Models.ResourceCreate()
            {
                Region = description.region,
                SpaceId = spaceId.ToString(),
                Type = description.type,
            };
    }
}