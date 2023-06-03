using UnityEngine;
using Cinemachine;
using System.Collections;
using ReplayEditor;
using UnityEngine.Rendering.HighDefinition;
using GameManagement;
using System.Collections.Generic;
using ModIO.UI;
using XLWeather.Utils;
using XLWeather.Data;

namespace XLWeather.Controller
{
    public class DroneController : MonoBehaviour
    {
        //public static DroneController Instance { get; set; }

        //public GameObject dronePrefab;
        //public GameObject activeDrone;
        private Light droneLight;
        private HDAdditionalLightData droneLightdata;
        public CinemachineVirtualCamera DroneLightCam;
        private CinemachinePOV DroneCamPov;
        private AudioSource droneAudio;

        private RaycastHit rayHit;
        private Vector3 offset;
        private Vector3 droneAnchor;

        /*
        public IEnumerator routine;
        Color LastColor;
        private IEnumerator ColorLerp()
        {
            while (true)
            {
                var startColor = LastColor;
                var endColor = new Color32(System.Convert.ToByte(Random.Range(0, 255)), System.Convert.ToByte(Random.Range(0, 255)), System.Convert.ToByte(Random.Range(0, 255)), 255);
                LastColor = endColor;

                float t = 0;
                while (t < 1)
                {
                    t = Mathf.Min(1, t + Time.deltaTime);
                    droneLight.color = Color.Lerp(startColor, endColor, t);
                    yield return null;
                }

                yield return new WaitForSeconds(1);
            }
        }
        */

        private void Start()
        {
            StartCoroutine(GetDroneObjects());
            //routine = ColorLerp();
            ResetDataSettings();
        }

        private void Update()
        {
            if (!ToggleStateData.DroneToggle)
                return;

            UpdateSettings();
        }

        private void FixedUpdate()
        {
            if (!ToggleStateData.DroneToggle)
                return;

            DroneRayCast();
            if (Main.Weatherctrl.state != typeof(ReplayState) | Main.Weatherctrl.state != typeof(PinMovementState))
            {
                if (!ToggleStateData.MultiTargetToggle)
                {
                    //SetAnchorPos();
                    SetDronePos();
                }

            }
        }

        private void LateUpdate()
        {
            if (!ToggleStateData.DroneToggle)
                return;

            if (Main.Weatherctrl.state == typeof(ReplayState) || Main.Weatherctrl.state == typeof(PinMovementState))
            {
                if (!ToggleStateData.MultiTargetToggle)
                {
                    //SetAnchorPos();
                    SetDronePos();
                }

            }
            if (ToggleStateData.MultiTargetToggle)
            {
                SetMultiDronePos();
            }
        }
        
        //Vector3 pinpos;
        //Vector3 playerpos;
        /*
        public void SetAnchorPos()
        {
            droneAnchor = (Main.Weatherctrl.state == typeof(ReplayState) ? Main.Weatherctrl.board_replay.position : Main.Weatherctrl.board.position);
            //droneAnchor = (Main.Weatherctrl.state == typeof(ReplayState) ? Main.Weatherctrl.pelvis_replay.position : Main.Weatherctrl.pelvis.position);
            //pinpos = PlayerController.Instance.pinMover.gameObject.transform.position;
            //playerpos = getOnlinePlayer();
        }
        */
        private void SetDronePos()
        {
            if (AssetHandler.Instance.activeDrone == null)
                return;

            // track target object
            //droneLight.gameObject.transform.LookAt(droneAnchor.transform.position);
            //activeDrone.transform.LookAt(new Vector3(droneAnchor.transform.position.x, activeDrone.transform.position.y, droneAnchor.transform.position.z));

            if (Main.Weatherctrl.state == typeof(PinMovementState))
            {
                Vector3 pinpos = PlayerController.Instance.pinMover.gameObject.transform.position; // 1.2.2.8
                //Vector3 pinpos = PlayerController.Main.pinController.gameObject.transform.position; // 1.2.6.0
                Dronefollow(pinpos);
            }
            else
            {
                //droneAnchor = (Main.Weatherctrl.state == typeof(ReplayState) ? Main.Weatherctrl.pelvis_replay.position : Main.Weatherctrl.pelvis.position);
                if (Main.settings.DroneTargetState == "Skateboard")
                {
                    droneAnchor = (Main.Weatherctrl.state == typeof(ReplayState) ? Main.Weatherctrl.board_replay.position : Main.Weatherctrl.board.position);
                }
                if (Main.settings.DroneTargetState == "Player")
                {
                    droneAnchor = (Main.Weatherctrl.state == typeof(ReplayState) ? Main.Weatherctrl.pelvis_replay.position : Main.Weatherctrl.pelvis.position);
                }
                if (droneAnchor != null)
                {
                    Dronefollow(droneAnchor);
                }
            }

        }

        //public static float Smooth(float source, float target, float rate, float dt)
        //{
        //    return Mathf.Lerp(source, target, 1 - Mathf.Pow(rate, dt));
        //}
        private void Dronefollow(Vector3 target)
        {
            // smoothly follow target object
            AssetHandler.Instance.activeDrone.transform.position += ((target + offset) - AssetHandler.Instance.activeDrone.transform.position) * Main.settings.dronefollowSharpness;
            droneLight.gameObject.transform.parent.LookAt(target);

            // smoothly rotates drone to face target object
            Vector3 target_y = new Vector3(target.x, AssetHandler.Instance.activeDrone.transform.position.y, target.z);
            var targetRotation = Quaternion.LookRotation(target_y - AssetHandler.Instance.activeDrone.transform.position);
            AssetHandler.Instance.activeDrone.transform.rotation = Quaternion.Slerp(AssetHandler.Instance.activeDrone.transform.rotation, targetRotation, Main.settings.droneRotSpeed * Time.deltaTime);
        }
        private void SetMultiDronePos()
        {
            if (AssetHandler.Instance.activeDrone == null && !ToggleStateData.MultiTargetToggle)
                return;

            Vector3 playerpos = getOnlinePlayer();
            if (playerpos != null)
            {
                Dronefollow(playerpos);
            }
            else
            {
                Dronefollow(droneAnchor);
            }
        }

        private float oldRayDist;
        private void DroneRayCast()
        {
            if (oldRayDist != Main.settings.droneRayDistance)
            {
                float newrayDist = Main.settings.droneRayDistance + 1;
                offset.Set(0, newrayDist, 0);
                oldRayDist = Main.settings.droneRayDistance;
            }

            // keeps object above the ground at set height value
            if (Physics.Raycast(AssetHandler.Instance.activeDrone.transform.position, -transform.up, out rayHit, Mathf.Infinity))
            {
                //get the position where the ray hit the ground
                Vector3 pos = rayHit.point;

                //shoot a raycast up towards the object
                Ray upRay = new Ray(pos, AssetHandler.Instance.activeDrone.transform.position - pos);

                //get distance from ray origin
                Vector3 upDist = upRay.GetPoint(Main.settings.droneRayDistance);

                //interpolate its position
                AssetHandler.Instance.activeDrone.transform.position = Vector3.Lerp(AssetHandler.Instance.activeDrone.transform.position, upDist, Time.deltaTime);

            }
        }

        private void SpawnDrone()
        {
            AssetHandler.Instance.activeDrone = Instantiate(AssetHandler.Instance.dronePrefab);
            AssetHandler.Instance.activeDrone.transform.SetParent(Main.scriptManager.transform);
            AssetHandler.Instance.activeDrone.SetActive(false);  
        }

        private IEnumerator GetDroneObjects()
        {
            yield return new WaitUntil(() => AssetHandler.Instance.IsPrefabsSpawned());

            //Transform droneobj = Main.Dronectrl.activeDrone.transform.Find("Drone");
            //playerAnchor = droneobj.FindChildRecursively("PlayerAnchor");
            //droneAnchor = droneobj.FindChildRecursively("DroneAnchor");

            Light[] DL = AssetHandler.Instance.activeDrone.GetComponentsInChildren<Light>();

            foreach (Light drLight in DL)
            {
                // Get Spot Light
                if (drLight.name == "DroneLight")
                {
                    droneLight = drLight.GetComponent<Light>();
                    droneLightdata = droneLight.GetComponent<HDAdditionalLightData>();
                }
            }
            //DroneLightCam = activeDrone.transform.FindChildRecursively("DroneCam");
            
            CinemachineVirtualCamera[] drCams = AssetHandler.Instance.activeDrone.GetComponentsInChildren<CinemachineVirtualCamera>();
            foreach (CinemachineVirtualCamera cams in drCams)
            {
                // Get Spot Light
                if (cams.name == "DroneCam")
                {
                    DroneLightCam = cams;
                    DroneCamPov = DroneLightCam.gameObject.GetComponentInChildren<CinemachinePOV>();
                }
            }
            // Get drone Audio
            droneAudio = AssetHandler.Instance.activeDrone.GetComponentInChildren<AudioSource>();

        }
        public string[] getPlayerList()
        {
            string[] usernames = new string[MultiplayerManager.Instance.networkPlayers.Count];
            int i = 0;
            foreach (KeyValuePair<int, NetworkPlayerController> playerID in MultiplayerManager.Instance.networkPlayers)
            {
                if (playerID.Value /*&& !playerID.Value.IsLocal*/)
                {
                    usernames[i] = playerID.Key + ":" + playerID.Value.NickName;
                    i++;
                }
            }
            return usernames;
        }
        private Vector3 getOnlinePlayer()
        {
            if (Main.settings.multiplayer_target == "None")
                return droneAnchor;

            int index = int.Parse(Main.settings.multiplayer_target.Split(':')[0]);
            var activeplayer = MultiplayerManager.Instance.GetNextPlayer(index - 1);

            if (activeplayer == null /*&& !activeplayer.IsLocal*/)
                return droneAnchor;

            Vector3 targetpos = droneAnchor;

            if (!ReplayCheck())
            {
                if (Main.settings.DroneTargetState == "Skateboard")
                {
                    Vector3 boardpos = activeplayer.GetSkateboard().transform.position;
                    targetpos = boardpos;
                }
                if (Main.settings.DroneTargetState == "Player")
                {
                    Vector3 playerpos = activeplayer.GetBody().transform.position;
                    targetpos = playerpos;
                }
                if (activeplayer.State.Equals(SkaterXL.Multiplayer.NetworkPlayerStateEnum.Pin))
                {
                    Vector3 pinpos = activeplayer.GetPin().transform.position;
                    targetpos = pinpos;
                }
                if (activeplayer.State.Equals(SkaterXL.Multiplayer.NetworkPlayerStateEnum.Replay))
                {
                    Vector3 replaypos = activeplayer.replayPlaybackController.transformReference.boardTransform.position;
                    targetpos = replaypos;
                }
            }
            else if (ReplayCheck())
            {
                Vector3 replaypos = droneAnchor;
                targetpos = replaypos;
            }

            return targetpos;
        }

        public bool ReplayCheck()
        {
            if (Main.Weatherctrl.state == typeof(ReplayState))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ResetDataSettings()
        {
            DroneSettings = new DroneData.DroneLightSettings(0, 0, 0, 0, 0, Color.white);
        }

        private DroneData.DroneLightSettings DroneSettings = new DroneData.DroneLightSettings(Main.settings.droneLtIntesityFlt, Main.settings.droneLtRangeFlt, Main.settings.droneLtAngleFlt, Main.settings.droneLtRadiusFlt, Main.settings.droneLtDimmerFlt, Main.settings.DroneLightColor);
        private float oldVolume;
        private float oldCamFov;
        private void UpdateSettings()
        {
            if (droneLight != null)
            {
                if (DroneSettings.Intensity != Main.settings.droneLtIntesityFlt)
                {
                    droneLightdata.intensity = Main.settings.droneLtIntesityFlt;
                    DroneSettings.Intensity = Main.settings.droneLtIntesityFlt;
                }
                if (DroneSettings.Range != Main.settings.droneLtRangeFlt)
                {
                    droneLight.range = Main.settings.droneLtRangeFlt;
                    DroneSettings.Range = Main.settings.droneLtRangeFlt;
                }
                if (DroneSettings.Angle != Main.settings.droneLtAngleFlt)
                {
                    droneLight.spotAngle = Main.settings.droneLtAngleFlt;
                    DroneSettings.Angle = Main.settings.droneLtAngleFlt;
                }
                if (DroneSettings.Radius != Main.settings.droneLtRadiusFlt)
                {
                    droneLightdata.shapeRadius = Main.settings.droneLtRadiusFlt;
                    DroneSettings.Radius = Main.settings.droneLtRadiusFlt;
                }
                if (DroneSettings.Dimmer != Main.settings.droneLtDimmerFlt)
                {
                    droneLightdata.lightDimmer = Main.settings.droneLtDimmerFlt;
                    DroneSettings.Dimmer = Main.settings.droneLtDimmerFlt;
                }
                if (DroneSettings.LightColor != Main.settings.DroneLightColor)
                {
                    droneLight.color = Main.settings.DroneLightColor;
                    DroneSettings.LightColor = Main.settings.DroneLightColor;
                }

                // store old settings in LightSettings object
                DroneSettings = new DroneData.DroneLightSettings(Main.settings.droneLtIntesityFlt, Main.settings.droneLtRangeFlt, Main.settings.droneLtAngleFlt, Main.settings.droneLtRadiusFlt, Main.settings.droneLtDimmerFlt, Main.settings.DroneLightColor);
            }

            // Update Audio settings
            if (droneAudio != null)
            {
                if (oldVolume != Main.settings.droneVolume)
                {
                    droneAudio.volume = Main.settings.droneVolume;
                    oldVolume = Main.settings.droneVolume;
                }
            }
            
            // update Drone Cam
            if (DroneLightCam != null)
            {
                if (ToggleStateData.DroneCamtoggle)
                {
                    if (oldCamFov != Main.settings.droneCamFov)
                    {
                        DroneLightCam.m_Lens.FieldOfView = Main.settings.droneCamFov;
                        oldCamFov = Main.settings.droneCamFov;
                    }

                    if (Main.UIctrl.showUI)
                    {
                        DroneCamPov.m_HorizontalAxis.m_MinValue = 0f;
                        DroneCamPov.m_HorizontalAxis.m_MaxValue = 0f;
                        DroneCamPov.m_VerticalAxis.m_MinValue = 0f;
                        DroneCamPov.m_VerticalAxis.m_MaxValue = 0f;
                    }
                    else
                    {
                        DroneCamPov.m_HorizontalAxis.m_MinValue = -40f;
                        DroneCamPov.m_HorizontalAxis.m_MaxValue = 40f;
                        DroneCamPov.m_VerticalAxis.m_MinValue = -40f;
                        DroneCamPov.m_VerticalAxis.m_MaxValue = 40f;
                    }
                }          
            }           
        }
    }
}
