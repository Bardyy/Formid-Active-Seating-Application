<?php 

$con = mysqli_connect('localhost:8889','root','root','formidactiveseating');

//check for connection
if(mysqli_connect_errno()){
    echo "1";
    exit();

}

$username = $_POST["username"];
$passsword = $_POST["password"];

$hash = hash('sha256',$password);

$insertuserquery = " SELECT * FROM USER WHERE username = '" . $username . "'  'AND' 'password' = '" . $hash . "' ;";

$check = mysqli_query($con, $insertuserquery);

if (mysqli_num_rows($check) > 0){

    echo("sucess");
}else{

    echo("invalid");
}



 ?>