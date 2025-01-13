using Application.Services.BackgroundService.BackgroundTask;
using AutoMapper;
using Domain.Entity;
using Domain.Models.Game;
using Domain.Models.PlayGameModels;
using Infrastructure;

namespace Application.Services.BackgroundService.ServiceTask.Common
{
    public class CommonTask : ICommonTask
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IBackgroundTaskQueue _taskQueue;

        public CommonTask(IBackgroundTaskQueue taskQueue, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _taskQueue = taskQueue;
        }

        public void AddSaveHistoryPlayer(string roomId)
        {
            _taskQueue.QueueCommonWorkItem(async token =>
            {
                await SaveHistoryPlayer(roomId);
            });
        }

        private async Task SaveHistoryPlayer(string roomId)
        {
            var unitOfWork = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWork>();
            var mapper = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMapper>();

            // Save history player
            var leaderboards = await unitOfWork.LeaderboardRepository.GetAll(roomId);

            var players = mapper.Map<IEnumerable<HistoryPlay>>(leaderboards);

            await unitOfWork.HistoryPlayRepository.Add(players);

            // Update user
            var room = await unitOfWork.RoomGameRepository.GetById(roomId);

            if (room == null)
            {
                Console.WriteLine("Room not found");
                return;
            }

            await Task.WhenAll(
                            UpdateTotalHostAsync(room, unitOfWork), // Update user total play
                            UpdateTotalPlayAsync(leaderboards, room, unitOfWork) // update user total host
                    );


            if (await unitOfWork.SaveChangesAsync())
            {
                Console.WriteLine("Save history player success");
                return;
            }

            Console.WriteLine("Save history player failed");
        }

        private async Task UpdateTotalHostAsync(RoomGame room, IUnitOfWork unitOfWork)
        {
            var user = await unitOfWork.UserRepository.GetById(room.UserId);

            if (user == null)
            {
                Console.WriteLine("User not found");
                return;
            }

            user.TotalHost++;
            user.HostKetIds?.Append(room.KetId);

            await unitOfWork.UserRepository.Update(user);

            Console.WriteLine("Update host player success");

        }

        private async Task UpdateTotalPlayAsync(IEnumerable<PlayerGame> players, RoomGame room, IUnitOfWork unitOfWork)
        {

            foreach (var player in players)
            {
                var user = await unitOfWork.UserRepository.GetById(player.Id);

                if(user != null)
                {
                    user.TotalPlay++;
                    user.ParticipantKetIds?.Append(room.KetId);

                    await unitOfWork.UserRepository.Update(user);
                }

            }
        }

    }
}
