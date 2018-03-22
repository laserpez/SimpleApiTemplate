using MongoDB.Driver;

namespace ProjectAPI.Extensions
{
    internal static class MongoResultExtensions
    {
        public static OpResult ToOpResult(this UpdateResult updateResult)
        {
            return new OpResult {Matched = updateResult.MatchedCount, Updated = updateResult.ModifiedCount};
        }
    }
}