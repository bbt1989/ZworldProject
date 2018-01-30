#pragma strict
var xx = 0.0;
var yy = 0.0;
function Update ()
{
	xx += Input.GetAxis("Mouse X")*5; yy += Input.GetAxis("Mouse Y")*5; yy = Mathf.Clamp (yy, -180, 180);
	transform.localRotation = Quaternion.AngleAxis(xx, Vector3.up);
	transform.localRotation *= Quaternion.AngleAxis(yy, Vector3.left);
	transform.position += transform.forward*Input.GetAxis("Vertical");
	transform.position += transform.right*Input.GetAxis("Horizontal");
}
