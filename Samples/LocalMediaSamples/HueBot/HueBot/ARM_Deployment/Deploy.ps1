$subscriptionName="ed43f29e-6445-4614-9829-d92b5ac46d51"
$resourceGroupName="tests"
$keyvaultName="andrueastman-keys"
$parameterFilePath="C:\Users\v-anomon\Documents\microsoft-graph-comms-samples\Samples\LocalMediaSamples\HueBot\HueBot\ARM_Deployment\AzureDeploy.Parameters.json"
$templateFilePath="C:\Users\v-anomon\Documents\microsoft-graph-comms-samples\Samples\LocalMediaSamples\HueBot\HueBot\ARM_Deployment\AzureDeploy.json"
$secretID="https://andrueastman-keys.vault.azure.net/secrets/bot-andrueastman-com/8e652a9630e74be18f62cb783ea61585"

Connect-AzureRmAccount
Select-AzureRmSubscription -SubscriptionName $subscriptionName

Set-AzureRmKeyVaultAccessPolicy -VaultName $keyvaultName -EnabledForDeployment
New-AzureRmServiceFabricCluster -ResourceGroupName $resourceGroupName -SecretIdentifier $secretId -TemplateFile $templateFilePath -ParameterFile $parameterFilePath
