using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dymatic;

namespace Sandbox
{
    using Dymatic;

    public class Player : Entity
    {
        private TransformComponent m_Transform;
        private RigidBody2DComponent m_Rigidbody;

        public float Speed;
        public float Time = 0.0f;

        public float Float = 1.0f;
        public double Double;
        public char Char;
        public byte Byte;
        public short Short;
        public int Int = 2;
        public long Long = 2;
        public ushort UShort;
        public uint UInt;
        public ulong ULong;
        public Vector2 Vector2;
        public Vector3 Vector3;
        public Vector4 Vector4;
        public Entity Entity;

        protected override void OnCreate()
        {
            Console.WriteLine($"Player.OnCreate - {ID}");

            m_Transform = GetComponent<TransformComponent>();
            m_Rigidbody = GetComponent<RigidBody2DComponent>();
        }

        protected override void OnUpdate(float ts)
        {
            Time += ts;
            Log.Info($"Player.OnUpdate: {ts}");

            float speed = Speed;
            Vector3 velocity = Vector3.Zero;

            if (Input.IsKeyDown(KeyCode.W))
                velocity.Y = 1.0f;
            else if (Input.IsKeyDown(KeyCode.S))
                velocity.Y = -1.0f;

            if (Input.IsKeyDown(KeyCode.A))
                velocity.X = -1.0f;
            else if (Input.IsKeyDown(KeyCode.D))
                velocity.X = 1.0f;

            Entity cameraEntity = FindEntityByName("Camera");
            if (cameraEntity != null)
            {
                Camera camera = cameraEntity.As<Camera>();

                if (Input.IsKeyDown(KeyCode.Q))
                    camera.DistanceFromPlayer += speed * 2.0f * ts;
                else if (Input.IsKeyDown(KeyCode.E))
                    camera.DistanceFromPlayer -= speed * 2.0f * ts;
            }

            Entity textEntity = FindEntityByName("Delta Text");
            if (textEntity != null)
            {
                TextComponent textComponent = textEntity.GetComponent<TextComponent>();
                textComponent.Text = $"Delta: {ts}";
                Vector3 rotation = textEntity.Rotation;
                textEntity.Rotation = new Vector3(rotation.X, textEntity.Rotation.Y + ts * 5.0f, rotation.Z);
            }

            velocity *= speed * ts;

            m_Rigidbody.ApplyLinearImpulse(velocity.XY, true);
        }

    }

    public class Collidable : Entity
    {
        private bool on = false;
        protected override void OnCreate()
        {
            Input.SetTriggerEffect(0, GamepadButtonCode.RightTrigger, 0.5f, true, 1.0f, 1.0f, 1.0f, 0.2f);
            Input.SetGamepadRumble(0, 0.0f, 0.0f, 0.0f);
        }

        protected override void OnUpdate(float ts)
        {
            float rightTrigger = Input.GetGamepadAxis(0, GamepadAxisCode.RightTriggerAxis);
            Log.Warn($"{rightTrigger}");
            if (rightTrigger > 0.5f)
            {
                Input.SetGamepadRumble(0, rightTrigger * 0.5f + 0.5f, rightTrigger * 0.5f + 0.5f, 0.0f);
                Input.SetGamepadLED(0, new Vector3(on ? 1.0f : 0.0f, 0.0f, 0.0f));
                on = !on;
            }
            else
            {
                Input.SetGamepadRumble(0, 0.0f, 0.0f, 0.0f);
                Input.SetGamepadLED(0, new Vector3(0.0f, 0.0f, 0.0f));
            }
        }

        protected override void OnDestroy()
        {
            Input.SetGamepadRumble(0, 0.0f, 0.0f, 0.0f);
        }
    }

    public class MeshScript : Entity
    {
        public Mesh Mesh;
        public Animation Animation;
        public Scene Scene;
        public Audio Audio;
        public Material Material;
        //public Asset someRandomAsset;
        public int test;
        public Entity OtherEntity;
        public MeshScript TestObject;
        public float Speed = 1.0f;

        private MaterialInstance instance;
        private Vector3 currentColor;
        private Vector3 nextColor = new Vector3(0.0f);
        private float currentTime = 0.0f;

        protected override void OnCreate()
        {
            Log.Info($"{test}");
            StaticMeshComponent smc = base.AddComponent<StaticMeshComponent>();
            smc.Mesh = Mesh;
            
            instance = Asset.GetAsset<Material>("M_Color Instance.dymateriali").CreateInstance();
            smc.Materials[0] = instance;
            Log.Info($"Material handle is {smc.Materials[0].Handle}");

            Entity other = Entity.FindEntityByName("Target");
            if (Tag != "Target")
            {
                other.AddComponent<ScriptComponent>("Sandbox.MeshScript");
                ((MeshScript)Entity.FindEntityByName("Target")).Speed = 3000.0f;
                other.GetComponent<StaticMeshComponent>().Mesh = Mesh;
            }
        }

        protected override void OnUpdate(float ts)
        {
            if (currentTime == 0.0f)
            {
                System.Random random = new System.Random();
                currentColor = nextColor;
                nextColor = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            }

            currentTime += ts;

            instance.SetParameter("Color", Dymatic.Math.Mix(currentColor, nextColor, currentTime / Speed));

            if (currentTime > Speed)
            {
                currentTime = 0.0f;
            }
        }
    }

    public class SpriteClass : Entity
    {
        public SpriteRendererComponent spc;

        public Texture MainTexture;
        public Entity MainEntity;
        public Vector2 TestVector;

        protected override void OnCreate()
        {
            spc = GetComponent<SpriteRendererComponent>();
            MainTexture.Hold();
        }

        protected override void OnUpdate(float ts)
        {
            Log.Warn($"{TestVector}");

            // Test Unloading/reloading texture asset rapidly
            if (spc.Texture)
                spc.Texture = null;
            else
                spc.Texture = MainTexture;

            if (Input.IsKeyDown(KeyCode.W))
                spc.Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            else if (Input.IsKeyDown(KeyCode.S))
                spc.Color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            else if (Input.IsKeyDown(KeyCode.A))
                spc.Color = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
            else if (Input.IsKeyDown(KeyCode.D))
                spc.Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            if (Input.IsKeyDown(KeyCode.Equal))
                spc.TilingFactor = spc.TilingFactor + 0.05f;
            else if (Input.IsKeyDown(KeyCode.Minus))
                spc.TilingFactor = spc.TilingFactor - 0.05f;
        }
    }

    public class Rotator : Entity
    {
        public float RotationSpeed = 1.0f;

        protected override void OnUpdate(float ts)
        {
            Vector3 rotation = Rotation;
            Rotation = new Vector3(rotation.X, rotation.Y, rotation.Z + ts * RotationSpeed);
        }
    }

    public class RaycastEntity : Entity
    {
        protected override void OnUpdate(float ts)
        {
            Vector3 pos = GetComponent<TransformComponent>().Translation;
            Debug.ClearDebugLines();
            Debug.DrawDebugLine(pos, pos + new Vector3(5.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));

            RaycastHit[] hits = Physics.BoxShapeCastMultihit(pos, new Vector3(1.0f, 0.0f, 0.0f), 10.0f, new Vector3(10.0f), new Vector3(0.0f));

            Log.Info("Beginning Hit List...");
            foreach(RaycastHit hit in hits)
                if (hit.Entity != null)
                    Log.Info($"Hit Entity '{hit.Entity.Tag}' at {hit.Position.ToString()} at distance {hit.Distance} with normal {hit.Normal}");
        }
    }

    public class ColliderEntity : Entity
    {
        public int Value = -1;
        protected override void OnContact(Entity other, Vector3 hitPosition, Vector3 normal)
        {
            Debug.DrawDebugLine(hitPosition, hitPosition + normal, new Vector3(1.0f, 0.0f, 0.0f), 10000);

            if (other is ColliderEntity)
            {
                Log.Trace("Collider Hit");
                ((ColliderEntity)other).Value = 42;
            }

            Log.Info($"This is {Tag}. I collided with {other.Tag} at {hitPosition} with normal {normal}!");
        }

        protected override void OnContactPersisted(Entity other, Vector3 hitPosition, Vector3 normal)
        {
            Log.Info($"{Tag} persisted colliding with {other.Tag} at {hitPosition} with normal {normal}!");
        }

        protected override void OnContactRemoved(Entity other)
        {
            Log.Info($"Entity {Tag} is no longer in contact with {other.Tag}");
        }
    }

    public class SceneOpener : Entity
    {
        public Scene SceneToOpen;

        protected override void OnUpdate(float ts)
        {
            if (Input.IsKeyDown(KeyCode.V) && SceneToOpen != null)
                Scene.OpenScene(SceneToOpen);
        }
    }

    public class BoundsChecker : Entity
    {
        protected override void OnUpdate(float ts)
        {
            Log.Info("Begin");
            Entity[] entities = Physics.GetEntitiesInBounds(new Vector3(-100.0f), new Vector3(100.0f));

            foreach (Entity entity in entities)
                Log.Info(entity.Tag);
        }
    }

    public class Test : Entity
    {
        protected override void OnCreate()
        {
            Entity target = Entity.FindEntityByName("Pivot");
            AddComponent<DistanceConstraintComponent>(target, 5.0f, 7.0f);
        }
    }

    public class VideoPlayerEntity : Entity
    {
        public VideoPlayer VideoPlayer;
        public Entity Subtitles;

        protected override void OnCreate()
        {
            VideoPlayer.Hold();
        }

        protected override void OnUpdate(float ts)
        {
            VideoPlayer.Update(ts);

            if (Input.IsKeyDown(KeyCode.D1))
                VideoPlayer.SetTime(40.0f);

            Subtitles.GetComponent<TextComponent>().Text = VideoPlayer.GetSubtitle();
        }
    }

    public class ImagePainter : Entity
    {
        public VirtualTexture Target;

        protected override void OnCreate()
        {
            Target.Resize(new Vector2(1024, 1024));
            float[] data = new float[1024 * 1024 * 4];

            Random random = new Random();
            for (int i = 0; i < 1024 * 1024 * 4; i++)
                data[i] = (float)random.NextDouble();

            Target.SetData(data);
        }
    }

    public class CaptureEntity : Entity
    {
        public Entity Target1;
        public Entity Target2;
        private int init = 0;

        protected override void OnCreate()
        {
        }

        protected override void OnUpdate(float ts)
        {
            if (init++ == 2)
            {
                CaptureComponent cc = GetComponent<CaptureComponent>();
                cc.Target.Clear();
                cc.AddToMask(Target1);
                cc.AddToMask(Target2);
            }
        }
    }

    public class LightRotator : Entity
    {
        public float Speed = 0.2f;
        private float Time = 0.0f;

        protected override void OnUpdate(float ts)
        {
            Time += ts;
            Rotation = new Vector3(Rotation.X, Rotation.Y, Math.Lerp(-84.0f, -54.0f, Math.Sin((Time * Speed) * 180.0f) * 0.5f + 0.5f));
        }
    }

    public class Character : Entity
    {
        public Prefab Projectile;

        private Entity SpringArm;
        private Entity Model;
        private Entity Gun;
        private Vector3 CameraRotation;
        private float Sensitivity = 0.15f;

        protected override void OnCreate()
        {
            Input.SetMouseLocked(true);

            AddComponent<CharacterMovementComponent>();

            Entity[] children = GetChildren();

            Projectile = Asset.GetAsset<Prefab>("Prefabs/Ball.dyprefab");

            // Setup Spring Arm
            Model = children[0];
            SpringArm = children[1];
            Log.Info(SpringArm.Tag);
            CameraRotation = SpringArm.GetComponent<TransformComponent>().GetWorldRotation();

            SpringArmComponent springArm = SpringArm.GetComponent<SpringArmComponent>();
            springArm.ExcludeEntity(this);

            Gun = Model.GetChildren()[0];
        }

        protected override void OnUpdate(float ts)
        {
            CharacterMovementComponent cmc = GetComponent<CharacterMovementComponent>();

            if (cmc.IsFalling())
                Log.Info("Character is falling!");

            Vector2 mouseDelta = Input.GetMouseDelta();
            CameraRotation.X -= mouseDelta.Y * Sensitivity;
            CameraRotation.Y -= mouseDelta.X * Sensitivity;
            SpringArm.GetComponent<TransformComponent>().SetWorldRotation(CameraRotation);

            Vector3 horizontalRotation = new Vector3(0.0f, CameraRotation.Y, 0.0f);
            Vector3 forward = Math.GetForward(horizontalRotation);
            Vector3 right = Math.GetRight(horizontalRotation);

            if (Projectile != null && Input.IsMouseButtonPressed(MouseCode.ButtonLeft))
            {
                Entity projectile = Projectile.Instantiate(new Transform(Gun.GetComponent<TransformComponent>().GetWorldTranslation() + forward * 2.0f));
                Log.Warn(projectile.Translation.ToString());
                projectile.GetComponent<RigidBodyComponent>().AddForce(forward * 1000.0f);

                Entity snowCapture = Entity.FindEntityByName("Snow Capture");
                if (snowCapture != null)
                    snowCapture.GetComponent<CaptureComponent>().AddToMask(projectile);
            }

            Vector3 movement = new Vector3();
            if (Input.IsKeyDown(KeyCode.W))
                movement += forward;
            if (Input.IsKeyDown(KeyCode.A))
                movement -= right;
            if (Input.IsKeyDown(KeyCode.S))
                movement -= forward;
            if (Input.IsKeyDown(KeyCode.D))
                movement += right;

            cmc.SetMovementInput(movement);

            if (Input.IsKeyDown(KeyCode.Space))
                cmc.Jump();

            StaticMeshComponent smc = Model.GetComponent<StaticMeshComponent>();
            smc.AnimationPlayer.SetParameter("Falling", cmc.IsFalling());
            smc.AnimationPlayer.SetParameter("Speed", cmc.GetLinearVelocity().Length() / cmc.MaxWalkSpeed);
        }
    }
}