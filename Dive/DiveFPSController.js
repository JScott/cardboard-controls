#pragma strict



var inputMoveDirection:Vector3;
var inputJump=false;
var grounded=false;
var downmovement=0.1;
var velocity:Vector3;
public var max_speed=0.1;
public var max_speed_air=0.13;
public var max_speed_ground=0.1;
public var acceleration=10;
public var acceleration_air=10;
public var gravity=-0.18;
public var friction=0.8;
public var cameraObject:GameObject;
private var controller : CharacterController;
public var groundNormal:Vector3;
public var jumpspeed=0.16;
public var stopmovingup=false;
public var fallkillspeed=-0.38;
public var collisionFlags : CollisionFlags; 

public var ground_gameobject:GameObject=null;
public var last_ground_pos:Vector3;
private var jumpcommand=0;
public var floating=false;

public var autowalk:int=0;
public var inhibit_autowalk=1;
public var reload_once:int=0;


function Awake () {
	controller = GetComponent (CharacterController);
	Debug.Log("Controller Slopelimit"+controller.slopeLimit);
}

function toggle_autowalk(){
	if (autowalk==0)autowalk=1;
	else autowalk=0;

}

function JumpUp(){

jumpcommand=1;

}


function Start () {

reload_once=0;

}


function OnControllerColliderHit (hit : ControllerColliderHit) {


	
if (hit.normal.y > 0 && hit.moveDirection.y < 0) {

		groundNormal = hit.normal;
		grounded=true;	
		ground_gameobject=hit.gameObject;
		
		//print("Landed on: ground"+ground_gameobject.name);
		stopmovingup=false;
	
						
	}
	

	
}

	



var fadeduration = 2.0; // fade duration in seconds

function Die(){
  // create a GUITexture:
  var fade: GameObject = new GameObject();
  fade.AddComponent(GUITexture);
  // and set it to the screen dimensions:
  fade.guiTexture.pixelInset = Rect(0, 0, Screen.width, Screen.height);
  // set its texture to a black pixel:
  var tex = new Texture2D(1, 1);
  tex.SetPixel(0, 0, Color.black);
  tex.Apply();
  fade.guiTexture.texture = tex;
  // then fade it during duration seconds
  for (var alpha:float = 0.0; alpha < 1.0; ){
    alpha += Time.deltaTime / fadeduration;
    fade.guiTexture.color.a = alpha;
    yield;
  }
  // finally, reload the current level:
    if (reload_once==0){
    reload_once =1;
    var async : AsyncOperation = Application.LoadLevelAsync(Application.loadedLevel);
     yield async;
     
     }
     
}

				

function Update () {


if (velocity.y < fallkillspeed)Die();


Debug.DrawLine (transform.position, transform.position+groundNormal, Color.red);
//print("GroundNormal y" +groundNormal.y);

var directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
if (autowalk==1)directionVector=Vector3(0,0,1*inhibit_autowalk);
if (directionVector != Vector3.zero) {
		// Get the length of the directon vector and then normalize it
		// Dividing by the length is cheaper than normalizing when we already have the length anyway
		var directionLength = directionVector.magnitude;
		directionVector = directionVector / directionLength;
		
		// Make sure the length is no bigger than 1
		directionLength = Mathf.Min(1, directionLength);
		
		// Make the input vector more sensitive towards the extremes and less sensitive in the middle
		// This makes it easier to control slow speeds when using analog sticks
		directionLength = directionLength * directionLength;
		
		// Multiply the normalized direction vector by the modified length
		directionVector = directionVector * directionLength;
	}
		// Apply the direction to the CharacterMotor
	inputMoveDirection =  directionVector;
	inputJump = Input.GetButton("Jump");
  
  
 				

	if(jumpcommand==1){
		inputJump=true;	
		jumpcommand=0;
		}
	
	grounded=(collisionFlags & CollisionFlags.Below);
	
	if (floating){
	velocity.y=0.1;
	}
	if (inputJump&&grounded){
	velocity.y+=jumpspeed;
	velocity.z*=1.5;
	
	//print ("Jump");
	grounded=false;
	}
	
	
	if (grounded){
			velocity+=inputMoveDirection*acceleration*Time.deltaTime;
		}
		else
		{
			velocity+=inputMoveDirection*acceleration_air*Time.deltaTime;
		}
	
	var translation=Vector3(velocity.x,0,velocity.z);
	
	translation=Vector3.Lerp(Vector3(0,0,0),translation,friction);
	
	velocity.x=translation.x;
	velocity.z=translation.z;
	velocity.y = velocity.y+gravity*Time.deltaTime;
	//print ("Time deltatime "+Time.deltaTime);
	
	
	if (grounded)velocity.y=0;
	//print ("Vel y "+velocity.y);
	
	
	
	if (grounded)max_speed=max_speed_ground;
	if (!grounded)max_speed=max_speed_air;
	
	if (translation.magnitude>max_speed){
		translation=translation/translation.magnitude;
		translation=translation*max_speed;
		}
	velocity.x=translation.x;
	velocity.z=translation.z;
	translation.y=velocity.y;
	
	
	
	
	
	var yrotation_camera= Quaternion.Euler(0, cameraObject.transform.rotation.eulerAngles.y, 0);
	//transform.position+=yrotation_camera*translation;
	
	
	var platformdelta:Vector3;
	
	if(ground_gameobject!=null) {
	//print("ground object ungleich null");
	platformdelta=ground_gameobject.transform.position-last_ground_pos;
	}
	
	//if (!grounded)platformdelta=Vector3.zero;
	
	
	//MAKE A MOVE!
	
	collisionFlags=controller.Move(yrotation_camera*translation+platformdelta);
	
	
	if (collisionFlags & CollisionFlags.CollidedAbove)
	{
	
	if (stopmovingup==false){
		print ("ControllerColliderHit");	
		velocity.y=0;	
		stopmovingup=true;
	}
	
	}
	

	
	
	if (ground_gameobject!=null && !grounded){
	ground_gameobject=null;
	}

	

	if(ground_gameobject!=null)last_ground_pos=ground_gameobject.transform.position;	


}




function LateUpdate () {
//groundNormal=Vector3.zero;
}


function OnGUI () {
//	GUI.Label (Rect (10, 10, 400, 20), "isgrounded "+grounded+   " velocity.y "+velocity.y+" collsionbelow "+ (collisionFlags & CollisionFlags.Below) );

	
       var e : Event = Event.current;
		if (e.isKey) {
			Debug.Log("Detected key code: " + e.keyCode);
			GUI.Label (Rect (10, 10, 400, 20), "Keycode ="+ e.keyCode );

	
		}
	 
	
				
	}






function OnTriggerEnter( other : Collider ) {



if (other.name == "Float") {

floating=true; 	 
inhibit_autowalk=0.1;



}





}

function OnTriggerExit( other : Collider ) {



if (other.name == "Float") {

floating=false; 	 
inhibit_autowalk=1;


}





}






@script RequireComponent (CharacterController)
