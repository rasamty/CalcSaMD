output "dev_default_host_name" {
  value       = module.dev.default_host_name
  description = "DEV's own *.azurestaticapps.net address - works the moment terraform apply finishes, no domain or DNS needed."
}

output "qa_default_host_name" {
  value = module.qa.default_host_name
}

output "uat_default_host_name" {
  value = module.uat.default_host_name
}

output "prod_default_host_name" {
  value = module.prod.default_host_name
}

output "dev_api_key" {
  value       = module.dev.api_key
  description = "Deployment token for DEV. Phase 7 (Part 13.4) sets this as the AZURE_STATIC_WEB_APPS_API_TOKEN secret scoped to the DEV GitHub Environment - the same secret name is reused for every environment, each holding a different value (gh secret set AZURE_STATIC_WEB_APPS_API_TOKEN --env DEV)."
  sensitive   = true
}

output "qa_api_key" {
  value       = module.qa.api_key
  description = "Deployment token for QA - same AZURE_STATIC_WEB_APPS_API_TOKEN secret name, scoped to the QA environment (Part 13.4)."
  sensitive   = true
}

output "uat_api_key" {
  value       = module.uat.api_key
  description = "Deployment token for UAT - same AZURE_STATIC_WEB_APPS_API_TOKEN secret name, scoped to the UAT environment (Part 13.4)."
  sensitive   = true
}

output "prod_api_key" {
  value       = module.prod.api_key
  description = "Deployment token for PROD - same AZURE_STATIC_WEB_APPS_API_TOKEN secret name, scoped to the PROD environment (Part 13.4)."
  sensitive   = true
}

output "dev_custom_domain_validation_token" {
  value       = module.dev.custom_domain_validation_token
  description = "DNS TXT record value proving ownership of dev.<your-domain> - null until domain_name is set (Phase 7, Part 13.2)."
  sensitive   = true
}

output "qa_custom_domain_validation_token" {
  value       = module.qa.custom_domain_validation_token
  description = "DNS TXT record value proving ownership of qa.<your-domain> (Part 13.2)."
  sensitive   = true
}

output "uat_custom_domain_validation_token" {
  value       = module.uat.custom_domain_validation_token
  description = "DNS TXT record value proving ownership of uat.<your-domain> (Part 13.2)."
  sensitive   = true
}

output "prod_custom_domain_validation_token" {
  value       = module.prod.custom_domain_validation_token
  description = "DNS TXT record value proving ownership of the bare root domain itself - PROD gets no subdomain prefix (Part 13.2)."
  sensitive   = true
}
