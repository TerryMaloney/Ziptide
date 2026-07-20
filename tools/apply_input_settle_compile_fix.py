from pathlib import Path

path = Path("Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/PlayerRigPersistence.cs")
text = path.read_text(encoding="utf-8")
old = '''            catch (System.Exception ex)
            {
                string runtimeType;
                try { runtimeType = action.valueType != null ? action.valueType.FullName : string.Empty; }
                catch { runtimeType = "<unavailable>"; }
                failure = owner + " action=" + ActionPath(action)
                    + " expected=" + (action.expectedControlType ?? string.Empty)
                    + " runtime=" + runtimeType
                    + " enabled=" + action.enabled
                    + " reference=" + (property.reference != null)
                    + " exception=" + ex.GetType().Name + ":" + ex.Message;
                return false;
            }
'''
new = '''            catch (System.Exception ex)
            {
                failure = owner + " action=" + ActionPath(action)
                    + " expected=" + (action.expectedControlType ?? string.Empty)
                    + " consumer=Vector2"
                    + " enabled=" + action.enabled
                    + " reference=" + (property.reference != null)
                    + " exception=" + ex.GetType().Name + ":" + ex.Message;
                return false;
            }
'''
count = text.count(old)
if count != 1:
    raise SystemExit(f"Expected exactly one unsupported diagnostic block, found {count}")
text = text.replace(old, new, 1)
if "action.valueType" in text:
    raise SystemExit("Unsupported InputAction.valueType reference remains")
path.write_text(text, encoding="utf-8")
print(f"Patched {path}")
