using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtRankingLib
{
    public class Manager
    {
        public RankingIndices RankingIndices = new RankingIndices();


                
        // 새로운 랭킹 추가 혹은 기존 랭킹 ID 반환
        int AddOrGetRankingIdByName(string rankingName)
        {
            int RankingIndex = RankingIndices[rankingName].IndexId;
            return RankingIndex;
        }
        
        // 랭킹 Id 기준으로 해당 랭킹의 이름을 반환
        public string GetRankingNameById(int rankingId)
        {
            return RankingIndices[rankingId].IndexName;
        }

        // 랭킹 Id 기준으로 랭킹 정보를 반환
        public GetRankingInfo GetRankingInfo(int rankingId)
        {
            var Index = RankingIndices[rankingId];
            
            var returnValue = new GetRankingInfo()
            {
                Result = 0,
                Length = Index.Tree.Count,
                Direction = Index.SortingDirection,
                TopScore = 0,
                BottomScore = 0,
                MaxElements = -1,
                TreeHeight = -1
                //TreeHeight = Index.Tree.height
            };

            if (Index.Tree.Count > 0)
            {
                returnValue.TopScore = Index.Tree.FrontElement.ScoreValue;
                returnValue.BottomScore = Index.Tree.BackElement.ScoreValue;
            }

            return returnValue;
        }

        //
        public ElementInfo GetUserRanking(int rankingIndexId, uint userId)
        {
            int IndexPosition = -1;
            var UserScore = default(RankingIndices.UserScore);

            var Ranking = RankingIndices[rankingIndexId];
            
            try
            {
                UserScore = Ranking.GetUserScore(userId);
                IndexPosition = Ranking.Tree.GetItemPosition(UserScore);
            }
            catch
            {
            }

            if (IndexPosition == -1 || UserScore == null)
            {
                return new ElementInfo()
                {
                    Position = -1,
                    UserId = 0,
                    ScoreValue = 0,
                    ScoreTimeStamp = 0,
                };
            }
            else
            {
                return new ElementInfo()
                {
                    Position = IndexPosition,
                    UserId = UserScore.UserId,
                    ScoreValue = UserScore.ScoreValue,
                    ScoreTimeStamp = UserScore.ScoreTimeStamp,
                };
            }
        }

        //
        public List<ElementInfo> GetUserRankingList(int rankingIndexId, int offset, int getCount)
        {
            var returnValueList = new List<ElementInfo>();

            var RankingIndex = RankingIndices[rankingIndexId];
            int CurrentEntryOffset = offset;

            if (offset >= 0)
            {
                foreach (var UserScore in RankingIndex.GetRange(offset, getCount))
                {
                    returnValueList.Add(new ElementInfo()
                    {
                        Position = CurrentEntryOffset,
                        UserId = UserScore.UserId,
                        ScoreValue = UserScore.ScoreValue,
                        ScoreTimeStamp = UserScore.ScoreTimeStamp,
                    });
                    CurrentEntryOffset++;
                }
            }
            
            return returnValueList;
        }

        //
        public void AddOrUpdateUserScore(int rankingIndexId, UserScoreInfo userScore)
        {
            var Index = RankingIndices[rankingIndexId];

            Index.UpdateUserScore(
                UserId: userScore.UserId,
                ScoreTimeStamp: userScore.ScoreTimeStamp,
                ScoreValue: userScore.ScoreValue
            );
        }

        //
        public void RemoveUserScore(int rankingIndexId, uint userId)
        {
            var Index = RankingIndices[rankingIndexId];
            Index.Tree.Remove(Index.GetUserScore(userId));
        }

        //
        public int RemoveAllUserScore(int rankingIndexId)
        {
            int count = 0;

            var RankingIndex = RankingIndices[rankingIndexId];
            count = RankingIndex.Tree.Count;
            RankingIndex.RemoveAllItems();

            return count;
        }
    }


    public struct GetRankingInfo
    {
        public int Result;
        public int Length;
        public RankingIndices.SortingDirection Direction;
        public int TopScore;
        public int BottomScore;
        public int MaxElements;
        public int TreeHeight;
    }

    public struct ElementInfo
    {
        public int Position;
        public Int64 UserId;
        public int ScoreValue;
        public uint ScoreTimeStamp;
    }

    public struct UserScoreInfo
    {
        public Int64 UserId;
        public int ScoreValue;
        public uint ScoreTimeStamp;
    }

    
}
