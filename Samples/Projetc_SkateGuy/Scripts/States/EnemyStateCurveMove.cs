using UnityEngine;
using GrazerCore.Tool;
using SkateHero.GameElements.EnemyLogicData;
using GrazerCore.GameElements;
using GrazerCore.GameElements.States;

namespace SkateHero.GameElements.States.EnemyStates
{
    public class EnemyStateCurveMove : BasicState
    {
        private Enemy enemy = null;
        private EnemyTypeCurveMoveLogicData logicData = null;
        private Transform moveTarget = null;
        private Vector2 startPos = Vector2.zero;
        private Vector2 endPos = Vector2.zero;
        private Vector2 aidPosA = Vector2.zero;
        private Vector2 aidPosB = Vector2.zero;
        private float speedScale = 0;
        private float t = 0;

        #region Search and Attack
        private Launcher[] launchers = null;
        private LayerMask targetMask = 0;
        private Vector2 searchDir = Vector2.zero;
        private float searchLength = 0;
        private float attackTime = 0;
        private bool attacking = false;
        private bool searchTarget = false;
        private float attackTimeCounter = 0;
        #endregion

        public EnemyStateCurveMove(StateController _stateController, Enemy _enemy,
            Vector2 _startPos, EnemyTypeCurveMoveLogicData _logicData) : base(_stateController)
        {
            stateController = _stateController;
            enemy = _enemy;
            logicData = _logicData;
            moveTarget = enemy.MoveTarget;
            startPos = _startPos;
            endPos = startPos + logicData.CurveEndPos;
            aidPosA = logicData.CurveAidPosA;
            aidPosB = logicData.CurveAidPosB;
            speedScale = logicData.SpeedScale;
            launchers = enemy.Launchers;

            //  Search and attack logic
            targetMask = _logicData.TargetMask;
            searchDir = _logicData.SearchDirection;
            searchLength = _logicData.SearchLength;
            attackTime = _logicData.AttackTime;
            attacking = false;
            searchTarget = false;
            attackTimeCounter = attackTime;
        }

        public override void OnEnter()
        {
            var launcherCount = launchers.Length;
            for (int index = 0; index < launcherCount; ++index)
            {
                var launcher = launchers[index];
                launcher.AwakeLauncher();
            }
        }

        public override void OnExit()
        {
            var launcherCount = launchers.Length;
            for (int index = 0; index < launcherCount; ++index)
            {
                var launcher = launchers[index];
                launcher.StopLauncher();
            }
        }

        public override void Track()
        {
            if (!searchTarget)
            {
                var startPos = moveTarget.position;
                var endPos = startPos + (Vector3)searchDir * searchLength;
                var target =  Physics2D.Linecast(startPos, endPos, targetMask);
                if (target.collider != null)
                {
                    searchTarget = true;
                    attacking = true;
                }
            }

            if (attacking)
            {
                if (attackTimeCounter > 0)
                {
                    //  Do attack
                    var launcherCount = launchers.Length;
                    for (int index = 0; index < launcherCount; ++index)
                    {
                        var launcher = launchers[index];
                        launcher.HoldTrigger();
                    }
                } else if (attackTimeCounter <= 0)
                {
                    attacking = false;
                }
                attackTimeCounter -= Time.deltaTime;
            }


            t += Time.deltaTime * speedScale;
            if (t >= 1)
            {
                moveTarget.localPosition = LineLerp.CubicLerp(startPos, aidPosA, aidPosB, endPos, 1);
                SetToNextState();
            } else
            {
                moveTarget.localPosition = LineLerp.CubicLerp(startPos, aidPosA, aidPosB, endPos, t);
            } 
        }
    }
}
