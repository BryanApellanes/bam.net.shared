

For the certificate side of things, using the Windows SDK command console or Visual Studio Professional command console

- Use makecert.exe to create a certificate authority. Example:

```
        makecert -n "CN=vMargeCA" -r -sv vMargeCA.pvk vMargeCA.cer
```

- Use makecert.exe to create an SSL certificate

```
        makecert -sk vMargeSignedByCA -iv vMargeCA.pvk -n "CN=vMargeSignedByCA" -ic vMargeCA.cer vMargeSignedByCA.cer -sr localmachine -ss My
```

- Use MMC GUI to install CA in Trusted Authority store
- Use MMC GUI to install an SSL certificate in Personal store

- Bind certificate to IP address:port and application. Example:

```
        netsh http add sslcert ipport=0.0.0.0:8443 certhash=585947f104b5bce53239f02d1c6fed06832f47dc appid={df8c8073-5a4b-4810-b469-5975a9c95230}
```        

        The certhash is the thumbprint from your SSL certificate. You can find this using mmc. The appid is found in Visual Studio...usually in assembly.cs, look for the GUID value.
