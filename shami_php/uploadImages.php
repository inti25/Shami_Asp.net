<?php
if (isset($_POST['data']) && isset($_POST['name'])) {
    $name = $_POST['name'];
    $image = $_POST['data'];
    $path = './Images/' . $name;
    $actual_path = 'http://shami.96.lt/Images/' . $name;
    try {
        file_put_contents($path, base64_decode($image));
        echo 'ok' . $actual_path;

    } catch (Exception $e) {
        echo "";
    }
} else {
    echo "";
}
?>