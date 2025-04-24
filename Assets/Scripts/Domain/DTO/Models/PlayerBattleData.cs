using System;
using System.Collections.Generic;

using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.DTO.Enums;

namespace Assets.Scripts.Infrastructure.Models
{
    public class PlayerBattleData
    {
        public int? Id { get; set; }
        public Guid? SessionId { get; set; }
        public Guid BuildId { get; set; }
        public int CountMissedMoves { get; set; }
        public PlayerSide Side { get; set; }
        public int TeamNumber { get; set; }
        public bool ConfirmedDeployment { get; set; }
        public int ColumnsToDeployment { get; set; }

        public Hero Hero { get; set; }
        public List<Unit> Units { get; set; }

        public PlayerBattleData(
            int? id,
            Guid? sessionId,
            Guid buildId,
            int countMissedMoves,
            PlayerSide side,
            int teamNumber,
            bool confirmedDeployment,
            int columnsToDeployment)
        {
            Id = id;
            SessionId = sessionId;
            BuildId = buildId;
            CountMissedMoves = countMissedMoves;
            Side = side;
            TeamNumber = teamNumber;
            ConfirmedDeployment = confirmedDeployment;
            ColumnsToDeployment = columnsToDeployment;
        }
    }
}
