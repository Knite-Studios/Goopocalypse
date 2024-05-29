using System;
using System.Collections.Generic;
using Entity.Player;
using Mirror;
using Runtime;
using UnityEngine;

namespace Entity
{
    public class Rope : NetworkBehaviour
    {
        public float maxLenght = 1.0f;
        public float elasticity = 0.1f;
        public float damping = 0.1f;
        public LayerMask collisionLayers;
        public GameObject ropeSegmentPrefab;
        private readonly float _ropeSegLen = 0.25f;
        private readonly List<RopeSegment> _ropeSegments = new();
        private readonly int _segmentLength = 35;
        private readonly List<Transform> _segmentTransforms = new();

        private Transform _buddie;
        private Transform _fwend;
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            NetworkClient.RegisterHandler<PlayersListS2CNotify>(OnPlayerListReceived);
            _lineRenderer = gameObject.GetOrAddComponent<LineRenderer>();
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.positionCount = _segmentLength;
        }

        private void Update()
        {
            if (!_fwend || !_buddie) return;

            DrawRope();
        }

        private void FixedUpdate()
        {
            if (!_fwend || !_buddie) return;

            Simulate();
        }

        private void OnPlayerListReceived(PlayersListS2CNotify message)
        {
            // If there's only 1 player, we don't need to create a rope.
            if (message.players.Count == 1)
            {
                gameObject.SetActive(false);
                return;
            }

            foreach (var player in message.players)
            {
                if (player.connection != connectionToServer) continue;

                var controller = player.connection.identity.GetComponent<PlayerController>();
                switch (controller.playerRole)
                {
                    case PlayerRole.Fwend:
                        _fwend = controller.transform;
                        break;
                    case PlayerRole.Buddie:
                        _buddie = controller.transform;
                        break;
                    case PlayerRole.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (_fwend && _buddie) InitializeRope();
        }

        private void InitializeRope()
        {
            var ropeStartPoint = _fwend.position;

            for (var i = 0; i < _segmentLength; i++)
            {
                _ropeSegments.Add(new RopeSegment(ropeStartPoint));
                var segmentInstance = Instantiate(ropeSegmentPrefab, ropeStartPoint, Quaternion.identity);
                _segmentTransforms.Add(segmentInstance.transform);
                ropeStartPoint.y -= _ropeSegLen;
            }
        }

        private void Simulate()
        {
            var forceGravity = new Vector2(0f, -1.5f);

            for (var i = 1; i < _segmentLength; i++)
            {
                var segment = _ropeSegments[i];
                var velocity = segment.PosNow - segment.PosOld;
                segment.PosOld = segment.PosNow;
                segment.PosNow += velocity;
                segment.PosNow += forceGravity * Time.fixedDeltaTime;
                _ropeSegments[i] = segment;
            }

            for (var i = 0; i < 50; i++) ApplyConstraints();
        }

        private void ApplyConstraints()
        {
            var firstSegment = _ropeSegments[0];
            firstSegment.PosNow = _fwend.position;
            _ropeSegments[0] = firstSegment;

            var lastSegment = _ropeSegments[_segmentLength - 1];
            lastSegment.PosNow = _buddie.position;
            _ropeSegments[_segmentLength - 1] = lastSegment;

            for (var i = 0; i < _segmentLength - 1; i++)
            {
                var firstSeg = _ropeSegments[i];
                var secondSeg = _ropeSegments[i + 1];

                var dist = (firstSeg.PosNow - secondSeg.PosNow).magnitude;
                var error = Mathf.Abs(dist - _ropeSegLen);
                var changeDir = (firstSeg.PosNow - secondSeg.PosNow).normalized;

                var changeAmount = changeDir * error;
                if (i != 0)
                {
                    firstSeg.PosNow -= changeAmount * 0.5f;
                    _ropeSegments[i] = firstSeg;
                    secondSeg.PosNow += changeAmount * 0.5f;
                    _ropeSegments[i + 1] = secondSeg;
                }
                else
                {
                    secondSeg.PosNow += changeAmount;
                    _ropeSegments[i + 1] = secondSeg;
                }
            }
        }

        private void DrawRope()
        {
            var ropePositions = new Vector3[_segmentLength];
            for (var i = 0; i < _segmentLength; i++) ropePositions[i] = _ropeSegments[i].PosNow;
            _lineRenderer.positionCount = ropePositions.Length;
            _lineRenderer.SetPositions(ropePositions);
        }

        private struct RopeSegment
        {
            public Vector2 PosNow;
            public Vector2 PosOld;

            public RopeSegment(Vector2 pos)
            {
                PosNow = pos;
                PosOld = pos;
            }
        }
    }
}
