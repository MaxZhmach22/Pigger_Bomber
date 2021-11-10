using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class GridControllerView : MonoBehaviour
    {
        [field: SerializeField] public GameObject GridPointPrefab { get; private set; }
        [field: SerializeField] public Grid Grid { get; private set; }
        [field: SerializeField] public int Rows { get; private set; } = 9;
        [field: SerializeField] public int Columns { get; private set; } = 17;
        [field: SerializeField] public float OffsetX { get; private set; } = 0.12f;
    }
}
