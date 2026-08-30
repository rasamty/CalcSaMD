# One Static Web App, optionally with one custom domain bound to it.
# Called once per environment (DEV/QA/UAT/PROD) from the root module -
# this file itself never mentions any environment name, which is what
# "parameterized and reused across all four environments" (Part 10)
# means concretely: the same module, four different inputs.

resource "azurerm_static_web_app" "this" {
  name                = var.name
  resource_group_name = var.resource_group_name
  location            = var.location

  # Both default to "Free" - listed explicitly anyway so a future edit
  # can't silently upgrade this to the paid Standard tier unnoticed.
  sku_tier = "Free"
  sku_size = "Free"
}

# Only created once var.domain_name is actually supplied (Phase 7) - the
# count trick is Terraform's standard way to make a whole resource
# conditional. With domain_name left at "" (the default through Phase 4),
# this list has zero elements and nothing custom-domain-related exists
# yet at all.
resource "azurerm_static_web_app_custom_domain" "this" {
  count = var.domain_name != "" ? 1 : 0

  static_web_app_id = azurerm_static_web_app.this.id
  domain_name       = var.domain_name

  # dns-txt-token (not cname-delegation) on purpose: it works no matter
  # which registrar or DNS provider your domain actually uses, it's the
  # only option that works for a bare root/apex domain (PROD's case),
  # and using ONE validation method for all four environments is simpler
  # to reason about than mixing two (Part 10.3).
  validation_type = "dns-txt-token"
}
