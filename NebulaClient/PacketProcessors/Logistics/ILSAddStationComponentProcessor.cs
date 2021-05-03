﻿using NebulaModel.Attributes;
using NebulaModel.Networking;
using NebulaModel.Packets.Logistics;
using NebulaModel.Packets.Processors;
using NebulaModel.Logger;
using NebulaWorld.Logistics;

namespace NebulaClient.PacketProcessors.Logistics
{
    [RegisterPacketProcessor]
    public class ILSAddStationComponentProcessor: IPacketProcessor<ILSAddStationComponent>
    {
        public void ProcessPacket(ILSAddStationComponent packet, NebulaConnection conn)
        {
            Log.Info($"ILSAddStationComponentProcessor processing packet for planet {packet.PlanetId}, station {packet.StationId} with gId of {packet.StationGId}");
            
            using (ILSShipManager.PatchLockILS.On())
            {
                GalacticTransport galacticTransport = GameMain.data.galacticTransport;

                if (packet.PlanetId == GameMain.localPlanet.id)
                {
                    Log.Info("Adding local planet station");
                    // If we're on the same planet as the new station was created on, should be able to find
                    // it in our local PlanetTransport.stationPool
                    StationComponent stationComponent = GameMain.localPlanet.factory.transport.stationPool[packet.StationId];
                    galacticTransport.AddStationComponent(packet.PlanetId, stationComponent);
                }
                else
                {
                    Log.Info("Adding fake station");
                    // If we're not on the same planet as the new station was create on, we need to create a 
                    // "fake" station that we can put into the GalacticTransport.stationPool instead of a real on
                    ILSShipManager.CreateFakeStationComponent(packet.StationGId, packet.PlanetId, true);
                }
            }
        }
    }
}
