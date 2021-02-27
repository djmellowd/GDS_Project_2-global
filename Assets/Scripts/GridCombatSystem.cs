using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridCombatSystem : MonoBehaviour
{
    [SerializeField] private UnitGridCombat unitGridCombat;
    [SerializeField] private UnitGridCombat[] unitGridCombatArray;
    private UnitGridCombat unitGridCombat;
    private State state;
    private List<UnitGridCombat> blueTeamList;
    private List<UnitGridCombat> redTeamList;
    private int blueTeamActiveUnitIndex;
    private int redTeamActiveUnitIndex;
    private bool canMoveThisTurn;
    private bool canAttackThisTurn;

    private enum State
    {
        Normal,
        Waiting
    }

    private void Awake()
    {
        state = State.Normal;
    }

    private void Start()
    {
        blueTeamList = new List<UnitGridCombat>();
        redTeamList = new List<UnitGridCombat>();
        blueTeamActiveUnitIndex = -1;
        redTeamActiveUnitIndex = -1;

        foreach (UnitGridCombat unitGridCombat in unitGridCombatArray)
        {
            GameHandler_GridCombatSystem.Instance.GetGrid().GetGridObject(unitGridCombat.GetPosition()).SetUnitGridCombat(unitGridCombat);
            if (unitGridCombat.GetTeam() == UnitGridCombat.Team.Blue)
            {
                blueTeamList.Add(unitGridCombat);
            }
            else
            {
                redTeamList.Add(unitGridCombat);
            }
        }
        SelectNextActiveUnit();
        UpdateValidMovePositions();
    }

    private void SelectNextActiveUnit()
    {
        if (unitGridCombat == null || unitGridCombat.GetTeam() == UnitGridCombat.Team.Red)
        {
            unitGridCombat = GetNextActiveUnit(unitGridCombat.Team.Blue);
        }
        else
        {
            unitGridCombat = GetNextActiveUnit(unitGridCombat.Team.Red);
        }

        GameHandler_GridCombatSystem.Instance.SetCameraFollowPosition(unitGridCombat.GetPosition());
        canMoveThisTurn = true;
        canAttackThisTurn = true;
    }

    private UnitGridCombat GetNextActiveUnit(UnitGridCombat.Team team)
    {
        if (team == UnitGridCombat.Team.Blue)
        {
            blueTeamActiveUnitIndex = (blueTeamActiveUnitIndex + 1) % blueTeamList.Count;
            return blueTeamList[blueTeamActiveUnitIndex];
        }
        else
        {
            redTeamActiveUnitIndex = (redTeamActiveUnitIndex + 1) % redeamList.Count;
            return redTeamList[redTeamActiveUnitIndex];
        }
    }

    private void UpdateValidMovePositions()
    {
        GridPathfinding gridPathfinding = GameHandler_GridCombatSystem.Instance.gridPathfinding;

        grid.GetXY(unitGridCombat.GetPosition(), out int unitX, out int unitY);

        GameHandler_GridCombatSystem.Instance.GetMovementTilemap().SetAllTilemapSprite(MovementTilemap.TilemapObject.TilemapSprite.None);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                grid.GetGridObject(x, y).SetIsValidMovePosition(false);
            }
        }

        int maxMoveDistance = 5;
        for (int x = unitX - maxMoveDistance; x <= unitY + maxMoveDistance; x++)
        {
            for (int y = unitY - maxMoveDistance; y <= unitY + maxMoveDistance; y++)
            {
                if (gridPathfinding.IsWalkable(x, y))
                {
                    if (gridPathfinding.HasPath(unitX, unitY, x, y))
                    {
                        if (gridPathfinding.GetPath(unitX, unitY, x, y).Count <= maxMoveDistance)
                        {
                            GameHandler_GridCombatSystem.Instance.GetMovementTilemap().SetTilemapSprite(x, y, MovementTilemap.TilemapObject.TilemapSprite.Move);

                            grid.GetGridObject(x, y).SetIsValidMovePosition(true);
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Normal:
                if (Input.GetMouseButtonDown(0))
                {
                    Grid<GridObject> grid = GameHandler_GridCombatSystem.Instance.GetGrid();
                    GridObject gridObject = grid.GetGridObject(UtilsClass.GetMouseWorldPosition());

                    if (gridObject.GetUnitGridCombat() != null)
                    {
                        if (unitGridCombat.IsEnemy(gridObject.GetUnitGridCombat()))
                        {
                            if (unitGridCombat.CanAttackUnit(gridObject.GetUnitGridCombat()))
                            {
                                if (canAttackThisTurn)
                                {
                                    canAttackThisTurn = false;
                                    state = State.Waiting;
                                    unitGridCombat.AttackUnit(gridObject.GetUnitGridCombat(), () =>
                                    {
                                        state = State.Normal;
                                    });
                                }
                            }
                            else
                            {
                                CodeMonkey.CMDebug.TextPopupMouse("Cannot attack!");
                            }
                            break;
                        }
                        else
                        {
                            // nie przeciwnik
                        }
                    }
                    else
                    {
                        // nie ma tu jednostki
                    }
                    
                    if (gridObject.GetIsValidMovePosition())
                    {
                        if (canMoveThisTurn)
                        {
                            canMoveThisTurn = false;
                            state = State.Waiting;
                            grid.GetGridObject(unitGridCombat.GetPosition()).CleanUnitGridCombat();
                            gridObject.SetUnitGridCombat(unitGridCombat);
                            unitGridCombat.MoveTo(UtilsClass.GetMouseWorldPosition(), () =>
                            {
                                state = State.Normal;
                                TestTurnOver();
                            });
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ForceTurnOver();
                }
                break;
            case State.Waiting:
                break;
        }
    }

    private void TestTurnOver()
    {
        if (!canMoveThisTurn && !canAttackThisTurn)
        {
            ForceTurnOver();
        }
    }

    private void ForceTurnOver()
    {
        SelectNextActiveUnit();
        UpdateValidMovePositions();
    }

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int y;
        private bool isValidMovePosition;
        private UnitGridCombat unitGridCombat;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetIsValidMovePosition(bool set)
        {
            isValidMovePosition = set;
        }

        public bool GetIsValidMovePosition()
        {
            return isValidMovePosition;
        }

        public void SetUnitGridCombat(UnitGridCombat unitGridCombat)
        {
            this.unitGridCombat = unitGridCombat;
        }

        public void CleanUnitGridCombat()
        {
            SetUnitGridCombat(null);
        }

        public void GetUnitGridCombat()
        {
            return unitGridCombat;
        }
    }
}
