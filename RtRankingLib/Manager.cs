using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtRankingLib
{
    public class Manager
    {
        RankingIndices RankingIndices = new RankingIndices();


        /// <summary>
        /// 새로운 랭킹 타입 추가 혹은 기존 랭킹 타입 ID 반환
        /// 여러 종류의 랭킹 타입(예. 오름차순, 내림 차순, 다양한 점수별로)을 추가할 수 있다.
        /// 만약 유저 점수1와 유저 점수2 두 가지의 값을 각각 랭킹을 구할 때는 Manager 클래스를 추가하지 않고 이 함수를 사용해서 추가한다.
        /// </summary>
        /// <param name="isDescending">true 이면 내림차순</param>
        /// <param name="rankingName">타입 이름</param>
        /// <returns>랭킹 타입 Id</returns>
        public int AddOrGetRankingIdByName(bool isDescending, string rankingName)
        {
            var rankingNameSortType = "";

            if (isDescending)
            {
                rankingNameSortType = "-" + rankingName;
            }
            else
            {
                rankingNameSortType = "+" + rankingName;
            }
            
            int RankingIndex = RankingIndices[rankingNameSortType].IndexId;
            return RankingIndex;
        }

        /// <summary>
        /// 랭킹 Id 기준으로 해당 랭킹의 이름을 반환
        /// </summary>
        /// <param name="rankingId">랭킹 unique Id</param>
        /// <returns>랭킹 이름</returns>
        public string GetRankingNameById(int rankingId)
        {
            return RankingIndices[rankingId].IndexName;
        }

        /// <summary>
        /// 랭킹 Id 기준으로 랭킹 정보를 반환 
        /// </summary>
        /// <param name="rankingId">랭킹 unique Id</param>
        /// <returns></returns>
        public RankingTypeInfo GetRankingTypeInfo(int rankingId)
        {
            var Index = RankingIndices[rankingId];
            
            var returnValue = new RankingTypeInfo()
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

        /// <summary>
        /// 유저의 랭킹 정보 얻기 
        /// </summary>
        /// <param name="rankingId">랭킹 unique Id</param>
        /// <param name="userId">유저 unique Id</param>
        /// <returns>현재 순위 정보</returns>
        public ElementInfo GetUserRanking(int rankingId, Int64 userId)
        {
            int IndexPosition = -1;
            var UserScore = default(RankingIndices.UserScore);

            var Ranking = RankingIndices[rankingId];
            
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

        /// <summary>
        /// 지정 범위의 랭킹 정보 얻기 
        /// </summary>
        /// <param name="rankingId">랭킹 unique Id</param>
        /// <param name="offset">범위를 가져올 순위</param>
        /// <param name="getCount">가져올 수</param>
        /// <returns>현재 순위 리스트</returns>
        public List<ElementInfo> GetUserRankingList(int rankingId, int offset, int getCount)
        {
            var returnValueList = new List<ElementInfo>();

            var RankingIndex = RankingIndices[rankingId];
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

        /// <summary>
        /// 유저의 정보 추가 및 업데이트 
        /// </summary>
        /// <param name="rankingId">랭킹 unique Id</param>
        /// <param name="userScore">유저 점수 정보</param>
        public void AddOrUpdateUserScore(int rankingId, UserScoreInfo userScore)
        {
            var Index = RankingIndices[rankingId];

            Index.UpdateUserScore(
                UserId: userScore.UserId,
                ScoreTimeStamp: userScore.ScoreTimeStamp,
                ScoreValue: userScore.ScoreValue
            );
        }

        #region 보류
        // 정렬이 제대로 동작하지 않음. 유저의 랭킹 정보 삭제
        //public void RemoveUserScore(int rankingIndexId, Int64 userId)
        //{
        //    var Index = RankingIndices[rankingIndexId];
        //    Index.Tree.Remove(Index.GetUserScore(userId));
        //}

        // 정렬이 제대로 동작하지 않음. 모든 유저의 랭킹 정보 삭제
        //public int RemoveAllUserScore(int rankingIndexId)
        //{
        //    int count = 0;

        //    var RankingIndex = RankingIndices[rankingIndexId];
        //    count = RankingIndex.Tree.Count;
        //    RankingIndex.RemoveAllItems();

        //    return count;
        //}
        #endregion
    }


    public struct RankingTypeInfo
    {
        public int Result;
        public int Length;
        public RankingIndices.SortingDirection Direction;
        public Int64 TopScore;
        public Int64 BottomScore;
        public int MaxElements;
        public int TreeHeight;
    }

    public struct ElementInfo
    {
        public int Position;
        public Int64 UserId;
        public Int64 ScoreValue;
        public uint ScoreTimeStamp;
    }

    public struct UserScoreInfo
    {
        public Int64 UserId;
        public Int64 ScoreValue;
        public uint ScoreTimeStamp;
    }

    
}
