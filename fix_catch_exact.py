# FrmVistorias.cs
with open(r"Trabalho\FrmVistorias.cs", "r", encoding="utf-8", errors="ignore") as f:
    lines = f.readlines()

new_lines = []
for line in lines:
    if "catch (MongoDB.Driver.MongoConnectionException)" in line and "ex" not in line:
        # Check if next lines use ex
        new_lines.append(line.replace("catch (MongoDB.Driver.MongoConnectionException)", "catch (MongoDB.Driver.MongoConnectionException ex)"))
    else:
        new_lines.append(line)

with open(r"Trabalho\FrmVistorias.cs", "w", encoding="utf-8") as f:
    f.writelines(new_lines)

# frmSantos.cs
with open(r"Trabalho\frmSantos.cs", "r", encoding="utf-8", errors="ignore") as f:
    lines = f.readlines()

new_lines = []
for i, line in enumerate(lines):
    if line.strip() == "catch" and i + 2 < len(lines) and "ex.Message" in lines[i+2]:
        indent = line[:len(line) - len(line.lstrip())]
        new_lines.append(indent + "catch (Exception ex)\n")
    else:
        new_lines.append(line)

with open(r"Trabalho\frmSantos.cs", "w", encoding="utf-8") as f:
    f.writelines(new_lines)

print("Done exact catch fixes.")
