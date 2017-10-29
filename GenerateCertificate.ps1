
$keyName = "demo2.local"


#Create certificate and export it
$newCert = New-SelfSignedCertificate -KeySpec Signature -certstorelocation cert:\CurrentUser\my -dnsname $keyName -NotAfter ((Get-Date).AddYears(10))
$pwd = ConvertTo-SecureString -String "p@ssw0rd" -Force -AsPlainText # password is not important it is not stored later in azure or used
Export-PfxCertificate -cert ("cert:\CurrentUser\my\" + $newCert.Thumbprint) -FilePath ("c:\" + $keyName + ".pfx") -Password $pwd 

$pfxFilePath = "c:\" + $keyName + ".pfx"
$pwd = "p@ssw0rd"
$flag = [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::Exportable 
$collection = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2Collection  
$collection.Import($pfxFilePath, $pwd, $flag) 


#$pkcs12ContentType = [System.Security.Cryptography.X509Certificates.X509ContentType]::Pkcs12 
#$clearBytes = $collection.Export($pkcs12ContentType) 
#$fileContentEncoded = [System.Convert]::ToBase64String($clearBytes) 
#$secret = ConvertTo-SecureString -String $fileContentEncoded -AsPlainText –Force 
#$secretContentType = 'application/x-pkcs12' 



$rawCert = $collection.GetRawCertData()
$base64Cert = [System.Convert]::ToBase64String($rawCert)
$rawCertHash = $collection.GetCertHash()
$base64CertHash = [System.Convert]::ToBase64String($rawCertHash)
$KeyId = [System.Guid]::NewGuid().ToString()

$keyCredentials = 
'"keyCredentials": [
    {
      "customKeyIdentifier": "'+ $base64CertHash + '",
      "keyId": "' + $KeyId + '",
      "type": "AsymmetricX509Cert",
      "usage": "Verify",
      "value":  "' + $base64Cert + '"
     }
  ],'
$keyCredentials
$keyCredentials | clip.exe #copies to clipboard
