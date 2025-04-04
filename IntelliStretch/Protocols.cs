﻿using System.Xml.Serialization;
namespace IntelliStretch
{
    public class Protocols
    {
        public class GeneralSettings
        {
            // Constructor
            public GeneralSettings() { }

            // Properties
            public Joint Joint { get; set; }
            [XmlIgnore]
            public JointSide JointSide { get; set; }
            public int FlexionMax { get; set; }
            public int ExtensionMax { get; set; }
            public int ExtraRange { get; set; }
            public int ActiveFlexionMax { get; set; }
            public int ActiveExtensionMax { get; set; }
            [XmlIgnore]
            public int GameFlexionMax { get; set; }
            [XmlIgnore]
            public int GameExtensionMax { get; set; }
        }

        public class WristSettings
        {
            // Constructor
            public WristSettings() { }

            // Properties
            public Joint Joint { get; set; } = Joint.Wrist;
            [XmlIgnore]
            public JointSide Side { get; set; }
            public int FlexionMax { get; set; }
            public int ExtensionMax { get; set; }
            public int ActiveFlexionMax { get; set; }
            public int ActiveExtensionMax { get; set; }

        }

        public class MCPSettings
        {
            public MCPSettings() { }

            // Properties
            public Joint Joint { get; set; }= Joint.MCP;
            [XmlIgnore]
            public JointSide Side { get; set; }
            public int FlexionMax { get; set; }
            public int ExtensionMax { get; set; }
            public int ActiveFlexionMax { get; set; }
            public int ActiveExtensionMax { get; set; }
            
        }

        public class StretchingProtocol
        {
            // Constructor
            public StretchingProtocol() { }
            public StretchingProtocol(StretchingProtocol protocol)
            {
                // Copy constructor
                this.Level = protocol.Level;                
                this.Duration = protocol.Duration;
                this.HoldingTime = protocol.HoldingTime;
                this.FlexionVelocity = protocol.FlexionVelocity;
                this.ExtensionVelocity = protocol.ExtensionVelocity;
                this.FlexionTorque = protocol.FlexionTorque;
                this.ExtensionTorque = protocol.ExtensionTorque;
                this.FlexionTorqueMax = protocol.FlexionTorqueMax;
                this.ExtensionTorqueMax = protocol.ExtensionTorqueMax;
            }

            // Properties
            public int Level { get; set; }
            public int Duration { get; set; }
            public int HoldingTime { get; set; }
            public int FlexionVelocity { get; set; }
            public int ExtensionVelocity { get; set; }
            public int FlexionTorque { get; set; }
            public int ExtensionTorque { get; set; }
            public int FlexionTorqueMax { get; set; }
            public int ExtensionTorqueMax { get; set; }
        }

        public enum Joint
        {
            All = 0,  // All-in-one
            Ankle = 1, // Ankle only
            Elbow = 2, // Elbow only
            MCP = 3, // MCP only
            Wrist = 4, // Wrist only
            Hand = 5, // Hand version, MCP | Wrist
            Arm = 6,  // Arm version, Elbow | Wrist
            Knee = 8  // Knee version 
        }

        public enum JointSide
        {
            None = 0,
            Left = 1,
            Right = 2
        }

        public enum Direction
        {
            Horizontal = 0,
            Vertical = 1
        }

        public class Assistive
        {
            public Assistive() { }

            public int Level { get; set; }
            public int Velocity { get; set; }
            public int DelayTime { get; set; }
        }

        public class Resistive
        {
            public Direction ControlDirection { get; set; }
            public float ScalingFactor { get; set; }
            public bool IsNoLoading { get; set; }
            public int FlexionResistance { get; set; }
            public int ExtensionResistance { get; set; }
        }

        public class GameProtocol
        {
            // Constructor
            public GameProtocol() 
            {
                AssistiveMode = new Assistive();
                ResistiveMode = new Resistive();
            }

            // Properties
            public Assistive AssistiveMode { get; set; }
            public Resistive ResistiveMode { get; set; }

        }

        public class IntelliProtocol
        {
            public IntelliProtocol() 
            {
                System = new SystemSettings();
                General = new GeneralSettings();
                Wrist = new WristSettings();
                MCP = new MCPSettings();
                Stretching = new StretchingProtocol();
                Game = new GameProtocol();
            }

            public SystemSettings System { get; set; }
            public GeneralSettings General { get; set; }
            public WristSettings Wrist { get; set; }
            public MCPSettings MCP { get; set; }
            public StretchingProtocol Stretching { get; set; }
            public GameProtocol Game { get; set; }
        }

        public class SystemSettings
        {
            // System settings related to users
            public SystemSettings() { }

            public int SamplingRate { get; set; }
            public bool IsSavingData { get; set; }
        }

        public class System
        {
            // System settings related to device
            public System() { }

            public Joint Joint { get; set; }
            public bool HasSensor { get; set; }
        }
    }
}
