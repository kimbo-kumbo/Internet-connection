using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Net
{
    public static class Debugger 
    {
        private static Text _console;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnStart()
        {
            _console = GameObject.FindObjectsOfType<Text>().FirstOrDefault(t => t.name == "Console");
#if UNITY_EDITOR
            Debug.Log("Console not found");
#endif
        }

        public static void Log(object message)
        {
#if UNITY_EDITOR
          Debug.Log(message);            
#elif UNITY_STANDALONE && !UNITY_EDITOR
          _console.text += message;
#endif
        }

        public static byte[] SerializePlayerData(object data)
        {
            var player = (PlayerData)data;

            var array = new List<byte>(16);

            array.AddRange(BitConverter.GetBytes(player.posX));
            array.AddRange(BitConverter.GetBytes(player.posZ));
            array.AddRange(BitConverter.GetBytes(player.rotY));
            array.AddRange(BitConverter.GetBytes(player.hp));

            return array.ToArray();
        }

        public static object DeserializePlayerData(byte[] data)
        {
            return new PlayerData
            {
                posX = BitConverter.ToSingle(data, 0),
                posZ = BitConverter.ToSingle(data, 4),
                rotY = BitConverter.ToSingle(data, 8),
                hp = BitConverter.ToSingle(data, 12),
            };            
        }

        public static byte[] SerializeProjectileData(object data)
        {
            var projectile = (ProjectileData)data;

            var array = new List<byte>(8);

            array.AddRange(BitConverter.GetBytes(projectile.posX));
            array.AddRange(BitConverter.GetBytes(projectile.posZ));            

            return array.ToArray();
        }

        public static object DeserializeProjectileData(byte[] data)
        {
            return new ProjectileData
            {
                posX = BitConverter.ToSingle(data, 0),
                posZ = BitConverter.ToSingle(data, 4),                
            };
        }
    }
    public struct ProjectileData
    {
        public float posX;
        public float posZ;        

        public static ProjectileData Create(ProjectileController projectile)
        {
            return new ProjectileData
            {
                posX = projectile.transform.position.x,
                posZ = projectile.transform.position.z,                
            };
        }

        public void Set(ProjectileController projectile)
        {
            var vector = projectile.transform.position;
            vector.x = posX; vector.z = posZ;
            projectile.transform.position = vector;
        }
    }

    public struct PlayerData
    {
        public float posX;
        public float posZ;
        public float rotY;
        public float hp;

        public static PlayerData Create(PlayerController player)
        {
            return new PlayerData
            {
                posX = player.transform.position.x,
                posZ = player.transform.position.z,
                rotY = player.transform.rotation.y,
                hp = player.Health
            };
        }

        public void Set(PlayerController player)
        {
            var vector = player.transform.position;
            vector.x = posX; vector.z = posZ;
            player.transform.position = vector;

            vector = player.transform.eulerAngles;
            vector.y = rotY;
            player.transform.eulerAngles = vector;
            player.Health = hp;
        }

    }
}
