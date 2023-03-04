<?php 

$con = mysqli_connect('localhost','root','root','FormidActiveSeating');

//check for connection
if(mysqli_connect_errno()){
    echo "1";
    exit();

}

$username = $_POST["username"];
$passsword = $_POST["password"];

$insertuserquery = " SELECT * FROM USER WHERE username = '" . $username . "'  'AND' 'password' = '" . $password . "' ;";

$check = mysqli_query($con, $insertuserquery);

if (mysqli_num_rows($check) > 0){

    echo("sucess");
}else{

    echo("invalid");
}



 ?>