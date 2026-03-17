import svgPaths from "./svg-rzg3juakjh";
import imgFinnivoRegisteredLogoWithIconColour1 from "figma:asset/86928f186f2df1850c5f82ac6f9a342fa7d7d90f.png";
import imgImage120 from "figma:asset/a73b4201f4f6a507c28f4cd9883de4e40ed87d31.png";

function FinnivoLogo() {
  return (
    <div className="absolute bg-white h-[100px] left-[49.5px] overflow-clip rounded-[10px] top-[48.5px] w-[260px]" data-name="Finnivo Logo">
      <div className="absolute h-[124px] left-[23px] top-px w-[230px]" data-name="Finnivo Registered Logo with icon colour 1">
        <img alt="" className="absolute inset-0 max-w-none object-50%-50% object-cover pointer-events-none size-full" src={imgFinnivoRegisteredLogoWithIconColour1} />
      </div>
    </div>
  );
}

function TextH3Regular() {
  return (
    <div className="absolute h-[20px] left-[12px] overflow-clip top-[12px] w-[24px]" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[18px] text-nowrap text-white top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        FR
      </p>
    </div>
  );
}

function ApplicationIndicator() {
  return (
    <div className="absolute left-0 overflow-clip size-[45px] top-0" data-name="Application Indicator">
      <div className="absolute left-0 size-[45px] top-0">
        <div className="absolute inset-0" style={{ "--fill-0": "rgba(69, 110, 146, 1)" } as React.CSSProperties}>
          <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 45 45">
            <path d={svgPaths.p2fa43000} fill="var(--fill-0, #456E92)" id="Ellipse 1" />
          </svg>
        </div>
      </div>
      <TextH3Regular />
    </div>
  );
}

function TextH3Regular1() {
  return (
    <div className="absolute h-[23px] left-[57px] overflow-clip top-[11px] w-[225px]" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#676975] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Financial Reporting 2025
      </p>
    </div>
  );
}

function IconExpandCircleDown24X() {
  return (
    <div className="absolute left-[291px] size-[24px] top-[11px]" data-name="Icon - Expand Circle Down 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icon - Expand Circle Down 24x">
          <path d={svgPaths.p3025c280} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function ApplicationSelection() {
  return (
    <div className="absolute h-[45px] left-[19.5px] overflow-clip top-[184.5px] w-[315px]" data-name="Application Selection">
      <ApplicationIndicator />
      <TextH3Regular1 />
      <IconExpandCircleDown24X />
    </div>
  );
}

function ButtonOutline() {
  return (
    <div className="absolute box-border content-stretch flex gap-[10px] h-[35px] items-center justify-center left-0 px-[50px] py-[15px] right-0 rounded-[10px] top-0" data-name="Button/Outline">
      <div aria-hidden="true" className="absolute border-[#e0e0e0] border-[0.5px] border-solid inset-0 pointer-events-none rounded-[10px]" />
    </div>
  );
}

function IconsSearch24X() {
  return (
    <div className="absolute bottom-[5px] left-[12px] size-[24px]" data-name="Icons - Search 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icons - Search 24x">
          <path d={svgPaths.p128f3a00} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function TextBodyRegular() {
  return (
    <div className="absolute inset-[25.71%_9px_20%_40px] overflow-clip" data-name="Text Body - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#676975] text-[14px] text-nowrap top-0 tracking-[0.46px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Search
      </p>
    </div>
  );
}

function SearchText() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[249.5px] w-[315px]" data-name="Search Text">
      <ButtonOutline />
      <IconsSearch24X />
      <TextBodyRegular />
    </div>
  );
}

function TextLabelsRegular() {
  return (
    <div className="absolute h-[16px] left-[19.5px] overflow-clip top-[324.5px] w-[290px]" data-name="Text Labels - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#676975] text-[12px] text-nowrap top-0 tracking-[0.46px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Data Collection
      </p>
    </div>
  );
}

function TextH3Regular2() {
  return (
    <div className="absolute h-[23px] left-[40px] overflow-clip top-[6px] w-[194px]" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#092e50] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Import Data
      </p>
    </div>
  );
}

function IconExpandDown32X() {
  return (
    <div className="absolute bottom-[2.86%] left-[89.84%] right-0 top-[5.71%]" data-name="Icon - Expand Down 32x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 32 32">
        <g id="Icon - Expand Down 32x">
          <path d={svgPaths.p1f8cc000} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function IconImport24X() {
  return (
    <div className="absolute left-[5px] size-[24px] top-[6px]" data-name="Icon - Import 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icon - Import 24x">
          <path d={svgPaths.p3cfcdc00} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function MenuImportData() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[360.5px] w-[315px]" data-name="Menu - Import Data">
      <TextH3Regular2 />
      <IconExpandDown32X />
      <IconImport24X />
    </div>
  );
}

function TextH3Regular3() {
  return (
    <div className="absolute inset-[17.14%_25.71%_17.14%_12.7%] overflow-clip" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#092e50] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Packs
      </p>
    </div>
  );
}

function IconExpandDown32X1() {
  return (
    <div className="absolute bottom-[2.86%] left-[89.84%] right-0 top-[5.71%]" data-name="Icon - Expand Down 32x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 32 32">
        <g id="Icon - Expand Down 32x">
          <path d={svgPaths.p1f8cc000} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function IconPacks24X() {
  return (
    <div className="absolute aspect-[24/24] left-[1.59%] overflow-clip right-[90.79%] top-[6px]" data-name="Icon - Packs 24x">
      <div className="absolute inset-[16.67%]" data-name="Vector">
        <div className="absolute inset-0" style={{ "--fill-0": "rgba(9, 46, 80, 1)" } as React.CSSProperties}>
          <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 16 16">
            <path d={svgPaths.p1a5b8b00} fill="var(--fill-0, #092E50)" id="Vector" />
          </svg>
        </div>
      </div>
    </div>
  );
}

function MenuPacks() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[405.5px] w-[315px]" data-name="Menu - Packs">
      <TextH3Regular3 />
      <IconExpandDown32X1 />
      <IconPacks24X />
    </div>
  );
}

function TextLabelsRegular1() {
  return (
    <div className="absolute h-[16px] left-[19.5px] overflow-clip top-[480.5px] w-[287px]" data-name="Text Labels - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#676975] text-[12px] text-nowrap top-0 tracking-[0.46px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Custom Reports
      </p>
    </div>
  );
}

function TextH3Regular4() {
  return (
    <div className="absolute inset-[17.14%_25.71%_17.14%_12.7%] overflow-clip" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#092e50] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Xtra Reports
      </p>
    </div>
  );
}

function IconExpandDown32X2() {
  return (
    <div className="absolute bottom-[2.86%] left-[89.84%] right-0 top-[5.71%]" data-name="Icon - Expand Down 32x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 32 32">
        <g id="Icon - Expand Down 32x">
          <path d={svgPaths.p1f8cc000} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function IconXtraReports24X() {
  return (
    <div className="absolute aspect-[24/24] left-[1.59%] overflow-clip right-[90.79%] top-[6px]" data-name="Icon - Xtra Reports 24x">
      <div className="absolute inset-[16.67%]" data-name="Vector">
        <div className="absolute inset-0" style={{ "--fill-0": "rgba(9, 46, 80, 1)" } as React.CSSProperties}>
          <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 16 16">
            <path d={svgPaths.p372ec880} fill="var(--fill-0, #092E50)" id="Vector" />
          </svg>
        </div>
      </div>
    </div>
  );
}

function MenuXtraReports() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[516.5px] w-[315px]" data-name="Menu - Xtra Reports">
      <TextH3Regular4 />
      <IconExpandDown32X2 />
      <IconXtraReports24X />
    </div>
  );
}

function TextH3Regular5() {
  return (
    <div className="absolute inset-[17.14%_25.71%_17.14%_12.7%] overflow-clip" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#092e50] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Publisher
      </p>
    </div>
  );
}

function IconExpandDown32X3() {
  return (
    <div className="absolute bottom-[2.86%] left-[89.84%] right-0 top-[5.71%]" data-name="Icon - Expand Down 32x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 32 32">
        <g id="Icon - Expand Down 32x">
          <path d={svgPaths.p1f8cc000} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function IconPublisher24X() {
  return (
    <div className="absolute aspect-[24/24] left-[1.59%] overflow-clip right-[90.79%] top-[6px]" data-name="Icon - Publisher 24x">
      <div className="absolute inset-[12.5%_20.83%]" data-name="Vector">
        <div className="absolute inset-0" style={{ "--fill-0": "rgba(9, 46, 80, 1)" } as React.CSSProperties}>
          <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 14 18">
            <path d={svgPaths.p1eb29e00} fill="var(--fill-0, #092E50)" id="Vector" />
          </svg>
        </div>
      </div>
    </div>
  );
}

function MenuPublisher() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[561.5px] w-[315px]" data-name="Menu - Publisher">
      <TextH3Regular5 />
      <IconExpandDown32X3 />
      <IconPublisher24X />
    </div>
  );
}

function TextH3Regular6() {
  return (
    <div className="absolute inset-[17.14%_25.71%_17.14%_12.7%] overflow-clip" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#092e50] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Dashboard
      </p>
    </div>
  );
}

function IconExpandDown32X4() {
  return (
    <div className="absolute bottom-[2.86%] left-[89.84%] right-0 top-[5.71%]" data-name="Icon - Expand Down 32x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 32 32">
        <g id="Icon - Expand Down 32x">
          <path d={svgPaths.p1f8cc000} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function IconDashboard24X() {
  return (
    <div className="absolute aspect-[24/24] left-[1.59%] overflow-clip right-[90.79%] top-[6px]" data-name="Icon - Dashboard 24x">
      <div className="absolute inset-[20.83%]" data-name="Vector">
        <div className="absolute inset-0" style={{ "--fill-0": "rgba(9, 46, 80, 1)" } as React.CSSProperties}>
          <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 14 14">
            <path d={svgPaths.p3554b600} fill="var(--fill-0, #092E50)" id="Vector" />
          </svg>
        </div>
      </div>
    </div>
  );
}

function MenuDashboard() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[606.5px] w-[315px]" data-name="Menu - Dashboard">
      <TextH3Regular6 />
      <IconExpandDown32X4 />
      <IconDashboard24X />
    </div>
  );
}

function TextLabelsRegular2() {
  return (
    <div className="absolute h-[16px] left-[19.5px] overflow-clip top-[681.5px] w-[290px]" data-name="Text Labels - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#676975] text-[12px] text-nowrap top-0 tracking-[0.46px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        More Options
      </p>
    </div>
  );
}

function TextH3Regular7() {
  return (
    <div className="absolute inset-[17.14%_25.71%_17.14%_12.7%] overflow-clip" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#092e50] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Application Setup
      </p>
    </div>
  );
}

function Frame() {
  return (
    <div className="absolute left-0 size-[24px] top-0" data-name="Frame">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Frame">
          <path d={svgPaths.p10be4b80} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function IconSettings24X() {
  return (
    <div className="absolute aspect-[24/24] left-[1.59%] overflow-clip right-[90.79%] top-[6px]" data-name="Icon - Settings 24x">
      <Frame />
    </div>
  );
}

function MenuApplicationSetup() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[717.5px] w-[315px]" data-name="Menu - Application Setup">
      <TextH3Regular7 />
      <IconSettings24X />
    </div>
  );
}

function TextH3Regular8() {
  return (
    <div className="absolute inset-[17.14%_25.71%_17.14%_12.7%] overflow-clip" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#092e50] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Import Rules
      </p>
    </div>
  );
}

function IconImportRules24X() {
  return (
    <div className="absolute aspect-[24/24] left-[1.59%] overflow-clip right-[90.79%] top-[6px]" data-name="Icon - Import Rules 24x">
      <div className="absolute inset-[20.92%_12.42%_20.83%_12.5%]" data-name="Vector">
        <div className="absolute inset-0" style={{ "--fill-0": "rgba(9, 46, 80, 1)" } as React.CSSProperties}>
          <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 19 14">
            <path d={svgPaths.p121d0a00} fill="var(--fill-0, #092E50)" id="Vector" />
          </svg>
        </div>
      </div>
    </div>
  );
}

function MenuImportRules() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[762.5px] w-[315px]" data-name="Menu - Import Rules">
      <TextH3Regular8 />
      <IconImportRules24X />
    </div>
  );
}

function TextH3Regular9() {
  return (
    <div className="absolute inset-[17.14%_25.71%_17.14%_12.7%] overflow-clip" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#092e50] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Export Cubes
      </p>
    </div>
  );
}

function IconExportCube24X() {
  return (
    <div className="absolute aspect-[24/24] left-[1.59%] overflow-clip right-[90.79%] top-[calc(50%+0.5px)] translate-y-[-50%]" data-name="Icon - Export Cube 24x">
      <div className="absolute inset-[12.9%_16.67%]" data-name="Vector">
        <div className="absolute inset-0" style={{ "--fill-0": "rgba(9, 46, 80, 1)" } as React.CSSProperties}>
          <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 16 18">
            <path d={svgPaths.p17172b00} fill="var(--fill-0, #092E50)" id="Vector" />
          </svg>
        </div>
      </div>
    </div>
  );
}

function IconExpandDown32X5() {
  return (
    <div className="absolute bottom-[2.86%] left-[89.84%] right-0 top-[5.71%]" data-name="Icon - Expand Down 32x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 32 32">
        <g id="Icon - Expand Down 32x">
          <path d={svgPaths.p1f8cc000} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function MenuExportRules() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[807.5px] w-[315px]" data-name="Menu - Export Rules">
      <TextH3Regular9 />
      <IconExportCube24X />
      <IconExpandDown32X5 />
    </div>
  );
}

function Frame1() {
  return <div className="absolute left-[-2px] size-[24px] top-[2px]" data-name="Frame" />;
}

function TextH3Regular10() {
  return (
    <div className="absolute inset-[17.14%_19.37%_17.14%_12.7%] overflow-clip" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#092e50] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Supporting Documents
      </p>
    </div>
  );
}

function IconSupportingDocuments24X() {
  return (
    <div className="absolute left-[5px] size-[24px] top-[6px]" data-name="Icon - Supporting Documents 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icon - Supporting Documents 24x">
          <path d={svgPaths.pcac0800} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function MenuSupportingDocuments() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[852.5px] w-[315px]" data-name="Menu - Supporting Documents">
      <Frame1 />
      <TextH3Regular10 />
      <IconSupportingDocuments24X />
    </div>
  );
}

function TextH3Regular11() {
  return (
    <div className="absolute inset-[17.14%_26.42%_17.14%_12.58%] overflow-clip" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#676975] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Henk Labuschagne
      </p>
    </div>
  );
}

function IconUserAccount24X() {
  return (
    <div className="absolute aspect-[24/24] left-[1.57%] overflow-clip right-[90.88%] top-[6px]" data-name="Icon - User Account 24x">
      <div className="absolute inset-[12.5%]" data-name="Vector">
        <div className="absolute inset-0" style={{ "--fill-0": "rgba(103, 105, 117, 1)" } as React.CSSProperties}>
          <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 18 18">
            <path d={svgPaths.p20433200} fill="var(--fill-0, #676975)" id="Vector" />
          </svg>
        </div>
      </div>
    </div>
  );
}

function LoggedInUser() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[1034.5px] w-[318px]" data-name="Logged in User">
      <TextH3Regular11 />
      <IconUserAccount24X />
    </div>
  );
}

function TextH3Regular12() {
  return (
    <div className="absolute inset-[17.14%_25.71%_17.14%_12.7%] overflow-clip" data-name="Text H3 - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#092e50] text-[18px] text-nowrap top-0 tracking-[-0.27px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        System Settings
      </p>
    </div>
  );
}

function IconSystemSettings24X() {
  return (
    <div className="absolute aspect-[24/24] left-[1.59%] overflow-clip right-[90.79%] top-[6px]" data-name="Icon - System Settings 24x">
      <div className="absolute inset-[12.5%]" data-name="Vector">
        <div className="absolute inset-0" style={{ "--fill-0": "rgba(9, 46, 80, 1)" } as React.CSSProperties}>
          <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 18 18">
            <path d={svgPaths.p2d906500} fill="var(--fill-0, #092E50)" id="Vector" />
          </svg>
        </div>
      </div>
    </div>
  );
}

function IconExpandDown32X6() {
  return (
    <div className="absolute bottom-[2.86%] left-[89.84%] right-0 top-[5.71%]" data-name="Icon - Expand Down 32x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 32 32">
        <g id="Icon - Expand Down 32x">
          <path d={svgPaths.p1f8cc000} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function MenuSystemSettings() {
  return (
    <div className="absolute h-[35px] left-[19.5px] overflow-clip top-[897.5px] w-[315px]" data-name="Menu - System Settings">
      <TextH3Regular12 />
      <IconSystemSettings24X />
      <IconExpandDown32X6 />
    </div>
  );
}

function IconHomePage24X() {
  return (
    <div className="absolute left-[306.5px] size-[24px] top-[1040.5px]" data-name="Icon - HomePage 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icon - HomePage 24x">
          <path d={svgPaths.p89d4380} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function OpenMainMenu() {
  return (
    <div className="absolute bg-white border-[#e0e0e0] border-[0.5px] border-solid h-[1080px] left-[-0.5px] overflow-clip rounded-[10px] top-[-0.5px] w-[360px]" data-name="Open Main Menu">
      <FinnivoLogo />
      <ApplicationSelection />
      <SearchText />
      <TextLabelsRegular />
      <MenuImportData />
      <MenuPacks />
      <TextLabelsRegular1 />
      <MenuXtraReports />
      <MenuPublisher />
      <MenuDashboard />
      <TextLabelsRegular2 />
      <MenuApplicationSetup />
      <MenuImportRules />
      <MenuExportRules />
      <MenuSupportingDocuments />
      <LoggedInUser />
      <MenuSystemSettings />
      <IconHomePage24X />
    </div>
  );
}

function ButtonOutline1() {
  return <div className="absolute bg-[rgba(122,162,192,0.15)] box-border content-stretch flex gap-[10px] inset-0 items-center px-[50px] py-[15px] rounded-[10px]" data-name="Button/Outline" />;
}

function GroupedItem() {
  return (
    <div className="absolute h-[60px] left-[9.5px] overflow-clip top-[177.5px] w-[340px]" data-name="Grouped Item">
      <ButtonOutline1 />
    </div>
  );
}

function Component() {
  return <div className="absolute bg-[#7aa2c0] h-[50px] left-0 right-0 rounded-[10px] top-0" data-name />;
}

function IconExportOptions32X() {
  return (
    <div className="absolute right-[18px] size-[32px] top-[9px]" data-name="Icon - Export Options 32x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 32 32">
        <g id="Icon - Export Options 32x">
          <path d={svgPaths.p2cf7f620} fill="var(--fill-0, white)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function TabBarWithExport() {
  return (
    <div className="absolute h-[50px] left-[369.5px] overflow-clip top-[-0.5px] w-[1550px]" data-name="Tab Bar with Export">
      <Component />
      <IconExportOptions32X />
    </div>
  );
}

function ButtonOutline2() {
  return (
    <div className="absolute bg-white box-border content-stretch flex gap-[10px] h-[35px] items-center justify-center left-0 px-[50px] py-[15px] rounded-[10px] top-0 w-[180px]" data-name="Button/Outline">
      <p className="font-['Inter:Medium',sans-serif] font-medium leading-[normal] not-italic relative shrink-0 text-[#092e50] text-[14px] text-center text-nowrap tracking-[0.46px] whitespace-pre">Packs</p>
    </div>
  );
}

function Close() {
  return (
    <div className="absolute left-[136px] size-[16px] top-[12px]" data-name="close">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 16 16">
        <g></g>
      </svg>
    </div>
  );
}

function IconClose24X() {
  return (
    <div className="absolute left-[152px] size-[24px] top-[8px]" data-name="Icon - Close 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icon - Close 24x"></g>
      </svg>
    </div>
  );
}

function IconClose16X() {
  return (
    <div className="absolute left-[156px] size-[16px] top-[10px]" data-name="Icon - Close 16x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 16 16">
        <g id="Icon - Close 16x">
          <path d={svgPaths.p1cc32b00} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function IconMoreInfoHorizontal32X() {
  return (
    <div className="absolute left-[5px] size-[32px] top-[2px]" data-name="Icon - More info Horizontal 32x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 32 32">
        <g id="Icon - More info Horizontal 32x">
          <path d={svgPaths.p2dc60d00} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function SelectedTab() {
  return (
    <div className="absolute h-[35px] left-[389.5px] overflow-clip top-[8.5px] w-[180px]" data-name="Selected Tab">
      <ButtonOutline2 />
      <Close />
      <IconClose24X />
      <IconClose16X />
      <IconMoreInfoHorizontal32X />
    </div>
  );
}

function Component1() {
  return <div className="absolute bg-white h-[50px] left-[-0.5px] right-[-0.5px] rounded-[10px] top-[-0.5px]" data-name />;
}

function ButtonOutline3() {
  return (
    <div className="absolute box-border content-stretch flex gap-[10px] h-[35px] items-center justify-center left-0 px-[50px] py-[15px] right-0 rounded-[10px] top-0" data-name="Button/Outline">
      <div aria-hidden="true" className="absolute border-[#456e92] border-[0.5px] border-solid inset-0 pointer-events-none rounded-[10px]" />
      <p className="font-['Inter:Medium',sans-serif] font-medium leading-[normal] not-italic relative shrink-0 text-[#092e50] text-[14px] text-center text-nowrap tracking-[0.46px] whitespace-pre">Save</p>
    </div>
  );
}

function Group() {
  return (
    <div className="absolute contents left-0 right-0 top-0">
      <ButtonOutline3 />
    </div>
  );
}

function UnselectedButton() {
  return (
    <div className="absolute h-[35px] left-[1431.5px] overflow-clip top-[7.5px] w-[100px]" data-name="Unselected Button">
      <Group />
    </div>
  );
}

function ButtonOutline4() {
  return (
    <div className="absolute bg-[#7aa2c0] box-border content-stretch flex gap-[10px] h-[35px] items-center justify-center left-0 px-[50px] py-[15px] right-0 rounded-[10px] top-0" data-name="Button/Outline">
      <p className="font-['Inter:Medium',sans-serif] font-medium leading-[normal] not-italic relative shrink-0 text-[14px] text-center text-nowrap text-white tracking-[0.46px] whitespace-pre">Refresh</p>
    </div>
  );
}

function Button() {
  return (
    <div className="absolute h-[35px] left-[1311.5px] overflow-clip top-[7.5px] w-[100px]" data-name="Button">
      <ButtonOutline4 />
    </div>
  );
}

function IconVersion24X() {
  return (
    <div className="absolute left-[10.5px] size-[24px] top-[6.5px]" data-name="Icon - Version 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icon - Version 24x">
          <path d={svgPaths.p2d796180} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function Frame4() {
  return (
    <div className="absolute border-[#7aa2c0] border-[0.5px] border-solid h-[35px] left-0 overflow-clip rounded-[10px] top-0 w-[174px]">
      <IconVersion24X />
    </div>
  );
}

function IconExpandDown24X() {
  return (
    <div className="absolute left-[142px] size-[24px] top-[6px]" data-name="Icon - Expand Down 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icon - Expand Down 24x">
          <path d={svgPaths.pf8180} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function Group5() {
  return (
    <div className="absolute contents left-0 top-0">
      <Frame4 />
      <IconExpandDown24X />
    </div>
  );
}

function TextBodyRegular1() {
  return (
    <div className="absolute h-[16px] left-[40px] overflow-clip top-[10px] w-[97px]" data-name="Text Body - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#676975] text-[14px] text-nowrap top-0 tracking-[0.46px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Select Version
      </p>
    </div>
  );
}

function FilterVersion() {
  return (
    <div className="absolute h-[35px] left-[1109.5px] overflow-clip top-[6.5px] w-[174px]" data-name="Filter - Version">
      <Group5 />
      <TextBodyRegular1 />
    </div>
  );
}

function Frame3() {
  return <div className="absolute border-[#7aa2c0] border-[0.5px] border-solid h-[35px] left-0 rounded-[10px] top-0 w-[174px]" />;
}

function IconPeriod24X() {
  return (
    <div className="absolute left-[11px] size-[24px] top-[6px]" data-name="Icon - Period 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icon - Period 24x">
          <path d={svgPaths.pc7ab8c0} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function IconExpandDown24X1() {
  return (
    <div className="absolute left-[141px] size-[24px] top-[6px]" data-name="Icon - Expand Down 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icon - Expand Down 24x">
          <path d={svgPaths.pf8180} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function Group4() {
  return (
    <div className="absolute contents left-0 top-0">
      <Frame3 />
      <IconPeriod24X />
      <IconExpandDown24X1 />
    </div>
  );
}

function Group1() {
  return (
    <div className="absolute contents left-0 top-0">
      <Group4 />
    </div>
  );
}

function TextBodyRegular2() {
  return (
    <div className="absolute h-[16px] left-[40px] overflow-clip top-[10px] w-[97px]" data-name="Text Body - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#676975] text-[14px] text-nowrap top-0 tracking-[0.46px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Select Period
      </p>
    </div>
  );
}

function FilterPeriod() {
  return (
    <div className="absolute h-[35px] left-[914.5px] overflow-clip top-[6.5px] w-[174px]" data-name="Filter - Period">
      <Group1 />
      <TextBodyRegular2 />
    </div>
  );
}

function IconExpandDown24X2() {
  return (
    <div className="absolute left-[316.5px] size-[24px] top-[5.5px]" data-name="Icon - Expand Down 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icon - Expand Down 24x">
          <path d={svgPaths.pf8180} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function IconEntity24X() {
  return (
    <div className="absolute left-[188.5px] size-[21.166px] top-[6.5px]" data-name="Icon - Entity 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 22 22">
        <g id="Icon - Entity 24x">
          <path d={svgPaths.p344aa300} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function TextBodyRegular3() {
  return (
    <div className="absolute inset-[calc(27.7%-0.5px)_calc(6%-0.5px)_calc(26.59%-0.5px)_calc(63.43%-0.5px)] overflow-clip" data-name="Text Body - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#676975] text-[14px] text-nowrap top-0 tracking-[0.46px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Select Entity
      </p>
    </div>
  );
}

function Frame2() {
  return (
    <div className="absolute border-[#7aa2c0] border-[0.5px] border-solid h-[35px] left-0 overflow-clip rounded-[10px] top-0 w-[350px]">
      <IconExpandDown24X2 />
      <IconEntity24X />
      <TextBodyRegular3 />
    </div>
  );
}

function Group3() {
  return (
    <div className="absolute contents left-0 top-0">
      <Frame2 />
    </div>
  );
}

function IconStructure24X() {
  return (
    <div className="absolute left-[13px] size-[16.885px] top-[10.45px]" data-name="Icon - Structure 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 17 17">
        <g id="Icon - Structure 24x">
          <path d={svgPaths.p20679600} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function Group2() {
  return (
    <div className="absolute contents left-0 top-0">
      <Group3 />
      <IconStructure24X />
    </div>
  );
}

function TextBodyRegular4() {
  return (
    <div className="absolute h-[15px] left-[42px] overflow-clip top-[10px] w-[135px]" data-name="Text Body - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#676975] text-[14px] text-nowrap top-0 tracking-[0.46px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Select Structure
      </p>
    </div>
  );
}

function FilterStructureAndEntityUnselected() {
  return (
    <div className="absolute h-[35px] left-[544.5px] overflow-clip top-[6.5px] w-[350px]" data-name="Filter - Structure and Entity = UNSELECTED">
      <Group2 />
      <TextBodyRegular4 />
    </div>
  );
}

function TextBodyRegular5() {
  return (
    <div className="absolute h-[16px] left-[39.5px] overflow-clip top-[9.5px] w-[97px]" data-name="Text Body - Regular">
      <p className="absolute font-['Roboto:Regular',sans-serif] font-normal leading-[normal] left-0 text-[#676975] text-[14px] text-nowrap top-0 tracking-[0.46px] whitespace-pre" style={{ fontVariationSettings: "'wdth' 100" }}>
        Select Pack
      </p>
    </div>
  );
}

function IconExpandDown24X3() {
  return (
    <div className="absolute left-[146.5px] size-[24px] top-[5.5px]" data-name="Icon - Expand Down 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 24 24">
        <g id="Icon - Expand Down 24x">
          <path d={svgPaths.pf8180} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function IconPacks24X1() {
  return (
    <div className="absolute left-[9.5px] size-[25.378px] top-[3.5px]" data-name="Icon - Packs 24x">
      <svg className="block size-full" fill="none" preserveAspectRatio="none" viewBox="0 0 26 26">
        <g id="Icon - Packs 24x">
          <path d={svgPaths.p3027f080} fill="var(--fill-0, #092E50)" id="Vector" />
        </g>
      </svg>
    </div>
  );
}

function FilterSelectPack() {
  return (
    <div className="absolute border-[#7aa2c0] border-[0.5px] border-solid h-[35px] left-[341.5px] overflow-clip rounded-[10px] top-[6.5px] w-[183px]" data-name="Filter - Select Pack">
      <TextBodyRegular5 />
      <IconExpandDown24X3 />
      <IconPacks24X1 />
    </div>
  );
}

function UfpConsolidationPacks() {
  return (
    <div className="absolute border-[#e0e0e0] border-[0.5px] border-solid h-[50px] left-[369.5px] overflow-clip rounded-[10px] top-[59.5px] w-[1550px]" data-name="UFP - Consolidation (Packs)">
      <Component1 />
      <UnselectedButton />
      <Button />
      <FilterVersion />
      <FilterPeriod />
      <FilterStructureAndEntityUnselected />
      <FilterSelectPack />
    </div>
  );
}

function ReportWithoutSelection() {
  return (
    <div className="absolute border border-[#e0e0e0] border-solid h-[960px] left-[369.5px] overflow-clip rounded-[10px] top-[119.5px] w-[1550px]" data-name="Report Without Selection">
      <div className="absolute inset-[-1px_calc(-12.69%-1px)_-1px_-1px]" data-name="image 120">
        <img alt="" className="absolute inset-0 max-w-none object-50%-50% object-cover pointer-events-none size-full" src={imgImage120} />
      </div>
    </div>
  );
}

export default function DesktopFrame1920X() {
  return (
    <div className="bg-[#eeeeee] border-[#e0e0e0] border-[0.5px] border-solid overflow-clip relative rounded-[10px] size-full" data-name="Desktop Frame 1920X1080">
      <OpenMainMenu />
      <GroupedItem />
      <TabBarWithExport />
      <SelectedTab />
      <UfpConsolidationPacks />
      <ReportWithoutSelection />
    </div>
  );
}