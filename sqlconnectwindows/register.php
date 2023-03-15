<?php 

$con = mysqli_connect('localhost:8889','root','root','formidactiveseating');

//check for connection
if(mysqli_connect_errno()){
	echo "1";
	exit();

}

$username = $_POST["username"];
$password = $_POST["password"];
$firstname = $_POST["firstname"];
$lastname = $_POST["lastname"];
$email = $_POST["email"];

$hashed_pass = hash('sha256', $password);

$checkuserquery = "SELECT * FROM USER WHERE username = '" . $username . "';";
$check = mysqli_query($con, $checkuserquery);

if(mysqli_num_rows($check) != 0){
	echo("User already exists!");
}else{
	$insertuserquery = "INSERT INTO User (username,pass,firstName,lastName,email) VALUES ( '" . $username . "' , '" . $hashed_pass . "' , '" . $firstname . " ', '" . $lastname . " ', '" . $email . "');";

	mysqli_query($con, $insertuserquery) or die("4: Insert user query failed");
	
	echo("0");
}



?>