using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class PerformanceTest
    {
        RtRankingLib.Manager RankingMgr = new RtRankingLib.Manager();

        public void 유저_추가하면서_정렬(int userCount)
        {
            Console.WriteLine("랭킹 성능 테스트 - 더미 데이터 생성 --->");
            
            var stopWatchWork = new System.Diagnostics.Stopwatch();
            stopWatchWork.Start();


            var random = new Random();

            var rankId= RankingMgr.AddOrGetRankingIdByName(true, "test1");

            for (int i = 0; i < userCount; ++i)
            {
                var userScore = new RtRankingLib.UserScoreInfo()
                {
                    UserId = i + 1,
                    ScoreValue = random.Next(1, 2000000000),
                };

                RankingMgr.AddOrUpdateUserScore(rankId, userScore);
            }

            stopWatchWork.Stop();
            Console.WriteLine("랭킹 성능 테스트 - 더미 데이터 생성 완료. 데이터 개수: {0}, 걸린 시간: {1}", userCount, stopWatchWork.Elapsed.TotalSeconds);

            

            Console.WriteLine("랭킹 성능 테스트 - 정렬 검증 --->");

            var rankingList = RankingMgr.GetUserRankingList(rankId, 0, userCount);

            for (int i = 1; i < userCount; i++)
            {
                if (rankingList[i - 0].ScoreValue < rankingList[i].ScoreValue)
                {
                    Console.WriteLine("랭킹 성능 테스트 - 정렬 실패. index: {0}, {1}", i - 1, i);
                    return;
                }
            }

            Console.WriteLine("랭킹 성능 테스트 - 정렬 검증 완료");





            Console.WriteLine("랭킹 성능 테스트 - 임의의 유저 업데이트 후 정렬 --->");
            
            var userScore1 = new RtRankingLib.UserScoreInfo()
            {
                UserId = random.Next(1, userCount),
                ScoreValue = random.Next(1, 2000000000),
            };

            var stopWatchWork1 = new System.Diagnostics.Stopwatch();
            stopWatchWork1.Start();

            RankingMgr.AddOrUpdateUserScore(rankId, userScore1);

            stopWatchWork1.Stop();
            Console.WriteLine("랭킹 성능 테스트 - 임의의 유저 업데이트 후 정렬. 걸린 시간: {0}", stopWatchWork1.Elapsed.TotalSeconds);


            Console.WriteLine("랭킹 성능 테스트 임의의 유저 업데이트 후 정렬- 정렬 검증 --->");

            rankingList = RankingMgr.GetUserRankingList(rankId, 0, userCount);

            for (int i = 1; i < userCount; i++)
            {
                if (rankingList[i - 0].ScoreValue < rankingList[i].ScoreValue)
                {
                    Console.WriteLine("랭킹 성능 테스트 임의의 유저 업데이트 후 정렬- 정렬 실패. index: {0}, {1}", i - 1, i);
                    return;
                }
            }

            Console.WriteLine("랭킹 성능 테스트 임의의 유저 업데이트 후 정렬 - 정렬 검증 완료");
        }
    }
}
